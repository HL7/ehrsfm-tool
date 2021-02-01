var fs = require('fs');
var xml2js = require('xml2js');
 
var parser = new xml2js.Parser();

/*
 This script will convert the PHR-S-FM R2 spreadsheet definition to a HL7-FM.

 2021-feb-01; corrected nesting of functions; added Section type='T'
 2021-jan-27; sort criteria > 100
 2020-dec-18; added sorting
 2020-dec-22; order of Sections fixed
 */
var rawxmlfp = fs.readFileSync('input/EHRSFM_R2_PHRSFM_R2_N1_2019JAN_Functionlist_20181203_20190123_20201027_MZ.xml');
parser.parseString(rawxmlfp, function (err, result) {
    var PID = "PHR_R2";
    var obj = {
        'model': {
            $: { 'xmlns': 'http://www.umcg.nl/MAX' },
            objects: {
                $: { 'xmlns': '' },
                object: [ {
                    id: PID, 
                    name: "PHR_R2",
                    alias: "",
                    stereotype: "HL7-FM",
                    type: "Package"
                } ]
            },
            relationships: {
                $: { 'xmlns': '' },
                relationship: [ ]
            }
        }
    };

    var sheet = result.Workbook.Worksheet.find(sheet => sheet.$['ss:Name'] === "Functions");
    var lastID = undefined;
    var fmidx = [];
    var rowno = 0
    var secno = 1;
    sheet.Table[0].Row.forEach(row => {
        rowno++;
        var ID = getCellValue(row, 0);
        var TYPE = getCellValue(row, 1);
        var NAME = getCellValue(row, 2);
        var STATEMENT = getCellValue(row, 5);
        var DESCRIPTION = getCellValue(row, 8);
        var CC = getCellValue(row, 12);
        var CRITERIA = getCellValue(row, 13);
        var FLAG = getCellValue(row, 19);

        var REF_ALIAS = getCellValue(row, 17);
        var REF_FUNCTION = getCellValue(row, 18);
        var REF_CC;
        if (REF_FUNCTION) {
            var sep_idx = REF_FUNCTION.indexOf(' ');
            if (sep_idx != -1) {
                REF_CC = REF_FUNCTION.substring(sep_idx + 1);
                REF_FUNCTION = REF_FUNCTION.substring(0, sep_idx);
            }
        }

        if (!ID) {
            ID = lastID;
        }
        else {
            lastID = ID;
        }
        //console.log(`${rowno} ${ID} ${TYPE} ${NAME} ${CC} ${CRITERIA} ${FLAG}`);

        // ignore rows marked as deleted
        if (FLAG == 'D') return;
        else if (!FLAG) {
            FLAG = 'NC';
        }

        switch (TYPE) {
            case 'T':
                fmidx[ID] = ID;
                // DESCRIPTION contains $EX$ and $AC$
                obj['model'].objects.object.push({ 
                    id: ID, 
                    name: NAME,
                    alias: ID,
                    notes: `$OV$${STATEMENT}${DESCRIPTION}`,
                    stereotype: "Section", 
                    type: "Package",
                    parentId: PID,
                    tag: [ { $: { name: "ID", value: secno++ } } ]
                });
                break;
            case 'H':
            case 'F':
                var parentSectionID = ID.substring(0,ID.lastIndexOf('.'));
                var _stereotype = (TYPE=='F'?"Function":"Header");
                var _type = "Feature";
                obj['model'].objects.object.push({ 
                    id: ID, 
                    name: `${ID} ${NAME}`,
                    alias: ID,
                    notes: `$ST$${STATEMENT}$DE$${DESCRIPTION}$EX$`,
                    stereotype: _stereotype, 
                    type: _type,
                    parentId: parentSectionID,
                    tag: [ { $: { name: "Row", value: rowno } },
                    { $: { name: "Reference.Alias", value: REF_ALIAS } },
                    { $: { name: "Reference.FunctionID", value: REF_FUNCTION } },
                    { $: { name: "Reference.ChangeIndicator", value: FLAG } } ]
                });
                fmidx[`${ID}`] = ID;
                break;
            case 'C':
                if (!CRITERIA) {
                    CRITERIA = JSON.stringify(row.Cell[13]['ss:Data'][0]);
                }
                var _name = formatID(ID, CC);
                var _stereotype = "Criteria";
                var _type = "Requirement";
                destId = fmidx[`${ID}`];
                if (!destId) {
                    console.error(`parent not found for ${ID}`);
                }
                var _optionality = '';
                if (CRITERIA.includes(" SHALL ")) _optionality = "SHALL";
                else if (CRITERIA.includes(" SHOULD ")) _optionality = "SHOULD";
                else if (CRITERIA.includes(" MAY ")) _optionality = "MAY";
                var _dependent = "N";
                if (CRITERIA.includes('according to scope of practice') ||
                    CRITERIA.includes('organizational policy') ||
                    CRITERIA.includes('jurisdictional law')
                ) _dependent = "Y";
                obj['model'].objects.object.push({ 
                    id: _name, 
                    name: _name,
                    notes: CRITERIA,
                    stereotype: _stereotype, 
                    type: _type,
                    parentId: destId,
                    tag: [ { $: { name: "Row", value: rowno } },
                        //{ $: { name: "Optionality", value: _optionality } },
                        { $: { name: "Conditional", value: CRITERIA.startsWith("IF ")?"Y":"N" } },
                        { $: { name: "Dependent", value: _dependent } },
                        { $: { name: "Reference.Alias", value: REF_ALIAS } },
                        { $: { name: "Reference.FunctionID", value: REF_FUNCTION } },
                        { $: { name: "Reference.CriterionID", value: REF_CC } },
                        { $: { name: "Reference.ChangeIndicator", value: FLAG } }
                    ]
                });
                break;
            default:
                break;
        }
    });

    // sort by FM ID
    var section_sortkey = [];
    section_sortkey['PH'] = '1PH';
    section_sortkey['S'] = '2S';
    section_sortkey['RI'] = '3RI';
    section_sortkey['TI'] = '4TI';

    obj['model'].objects.object.sort(function (a,b) {
        var aname = a.name;
        var bname = b.name;
        // Package should be first!
        if (a.stereotype == 'HL7-FM-Profile' || a.stereotype == 'HL7-FM') {
            aname = "0" + aname;
        }
        if (b.stereotype == 'HL7-FM-Profile' || a.stereotype == 'HL7-FM') {
            bname = "0" + bname;
        }

        if (a.stereotype == 'Section') {
            aname = section_sortkey[a.id];
        }
        else if (a.stereotype == 'Header' || a.stereotype == 'Function') {
            var p = a.alias.split(/[\. ]/);
            aname = section_sortkey[p[0]];
            for(var i=1; i<p.length; i++){
                aname += '.';
                var num = Number(p[i]);
                if (num<10) aname += '0';
                aname += num;
            }
        }
        else if (a.stereotype == 'Criteria') {
            var p = a.name.split(/[\.#]/);
            aname = section_sortkey[p[0]];
            for(var i=1; i<p.length-1; i++){
                aname += '.';
                var num = Number(p[i]);
                if (num<10) aname += '0';
                aname += num;
            }
            aname += '#';
            var cnum = Number(p[p.length-1]);
            if (cnum<10) aname += '00';
            else if (cnum<100) aname += '0';
            aname += cnum;
        }

        if (b.stereotype == 'Section') {
            bname = section_sortkey[b.id];
        }
        else if (b.stereotype == 'Header' || b.stereotype == 'Function') {
            var p = b.alias.split(/[\. ]/);
            bname = section_sortkey[p[0]];
            for(var i=1; i<p.length; i++){
                bname += '.';
                if (p[i]<10) bname += '0';
                bname += p[i];
            }
        }
        else if (b.stereotype == 'Criteria') {
            var p = b.name.split(/[\.#]/);
            bname = section_sortkey[p[0]];
            for(var i=1; i<p.length-1; i++){
                bname += '.';
                var num = Number(p[i]);
                if (num<10) bname += '0';
                bname += num;
            }
            bname += '#';
            var cnum = Number(p[p.length-1]);
            if (cnum<10) bname += '00';
            else if (cnum<100) bname += '0';
            bname += cnum;
        }
        return (aname > bname) ? 1 : -1 ;
    });

    var builder = new xml2js.Builder();
    console.log (builder.buildObject(obj))
});

function getCellValue(row, cellidx) {
    return row.Cell[cellidx] && row.Cell[cellidx].Data?row.Cell[cellidx].Data[0]._:undefined;
}

function formatID(ID, CC) {
    var cc = `${CC}`;
    if (cc.length < 2) cc = `0${cc}`;
    return `${ID}#${cc}`;
}
