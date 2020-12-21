var fs = require('fs');
const { abort } = require('process');
var xml2js = require('xml2js');
 
var parser = new xml2js.Parser();

/*
 It was decided to make the Usability FP a companion FP, so there is no compilation step required.
 This script will convert the definition to a HL7-FM-Profile.

 2020-dec-20; sort Profile/Section(s)/Header/Function/Criteria by ID en seq#
              TODO: order of Sections is now alfa, but should be xyz??!!
 */
var rawxmlfp = fs.readFileSync('input/EHRS_USEGUIDE_R1_I1_2019JAN_Usability_Functionlist_20190104_20200224b.xml');
parser.parseString(rawxmlfp, function (err, result) {
    var PID = "UFP";
    var obj = {
        'model': {
            $: { 'xmlns': 'http://www.umcg.nl/MAX' },
            objects: {
                $: { 'xmlns': '' },
                object: [ {
                    id: PID, 
                    name: "UFP",
                    alias: "",
                    stereotype: "HL7-FM-Profile", 
                    type: "Package",
                    tag: [ { $: { name: "PrioritiesDescription", value: "" } },
                        { $: { name: "Type", value: "Companion" } },
                        { $: { name: "Rationale", value: "<memo>" } },
                        { $: { name: "Version", value: "1.0" } },
                        { $: { name: "Scope", value: "<memo>" } },
                        { $: { name: "LanguageTag", value: "" } },
                        { $: { name: "PrioritiesDefinition", value: "<memo>" } },
                        { $: { name: "ConformanceClause", value: "<memo>" } } 
                    ]
                } ]
            },
            relationships: {
                $: { 'xmlns': '' },
                relationship: [ ]
            }
        }
    };

    var sheet = result.Workbook.Worksheet.find(sheet => sheet.$['ss:Name'] === "Usability_FP");
    var lastID = undefined;
    var fmidx = [];
    var CC = [];
    var rowno = 0
    var secno = 1;
    sheet.Table[0].Row.forEach(row => {
        rowno++;
        var ID = getCellValue(row, 6);
        var TYPE = getCellValue(row, 7);
        var NAME = getCellValue(row, 8);
        var STATEMENT = getCellValue(row, 10);
        if (!STATEMENT) STATEMENT = "";
        var DESCRIPTION = getCellValue(row, 12);
        if (!DESCRIPTION) DESCRIPTION = "";
        var CRITERIA = getCellValue(row, 15);
        if (!ID) {
            ID = lastID;
        }
        else {
            lastID = ID;
        }
        //console.log(`${ID} ${TYPE} ${NAME} ${CC} ${CRITERIA} ${FLAG}`);

        switch (TYPE) {
            case 'H':
            case 'F':
                var parentSectionID = ID.substring(0,ID.indexOf('.'));
                if(fmidx[parentSectionID] == undefined) {
                    // Create Section Placeolder
                    fmidx[parentSectionID] = parentSectionID;
                    obj['model'].objects.object.push({ 
                        id: parentSectionID, 
                        name: parentSectionID,
                        alias: parentSectionID,
                        stereotype: "Section", 
                        type: "Package",
                        parentId: PID,
                        tag: [ { $: { name: "ID", value: secno++ } } ]
                    });
                }

                var _stereotype = (TYPE=='F'?"Function":"Header");
                var _type = "Feature";
                obj['model'].objects.object.push({ 
                    id: rowno, 
                    name: `${ID} ${NAME}`,
                    alias: ID,
                    notes: `$ST$${STATEMENT}$DE$${DESCRIPTION}$EX$`,
                    stereotype: _stereotype, 
                    type: _type,
                    parentId: parentSectionID,
                    tag: [ { $: { name: "Row", value: rowno } },
                        { $: { name: "Reference.ChangeInfo", value: "N" } } ]
                });
                fmidx[`${ID}`] = rowno;
                break;
            case 'C':
                if (!CRITERIA) {
                    CRITERIA = JSON.stringify(row.Cell[15]['ss:Data'][0]);
                }
                if (!CC[`${ID}`]) { CC[`${ID}`] = 1; }
                else { CC[`${ID}`]++; }
                var _name = formatID(ID, CC[`${ID}`]);
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
                    id: rowno, 
                    name: _name,
                    notes: CRITERIA,
                    stereotype: _stereotype, 
                    type: _type,
                    parentId: destId,
                    tag: [ { $: { name: "Row", value: rowno } },
                        { $: { name: "Optionality", value: _optionality } },
                        { $: { name: "Conditional", value: CRITERIA.startsWith("IF ")?"Y":"N" } },
                        { $: { name: "Dependent", value: _dependent } },
                        { $: { name: "Reference.ChangeInfo", value: "N" } }
                    ]
                });
                break;
            default:
                break;
        }
    });

    // sort by FM ID
    obj['model'].objects.object.sort(function (a,b) {
        var aname = a.name;
        var bname = b.name;
        // Package should be first!
        if (a.stereotype == 'HL7-FM-Profile') {
            aname = "0" + aname;
        }
        if (b.stereotype == 'HL7-FM-Profile') {
            bname = "0" + bname;
        }

        if (a.stereotype == 'Section') {
            aname = a.id;
        }
        else if (a.stereotype == 'Header' || a.stereotype == 'Function') {
            var p = a.alias.split(/[\. ]/);
            aname = p[0];
            for(var i=1; i<p.length; i++){
                aname += '.';
                var num = Number(p[i]);
                if (num<10) aname += '0';
                aname += num;
            }
        }
        else if (a.stereotype == 'Criteria') {
            var p = a.name.split(/[\.#]/);
            aname = p[0];
            for(var i=1; i<p.length-1; i++){
                aname += '.';
                var num = Number(p[i]);
                if (num<10) aname += '0';
                aname += num;
            }
            aname += '#' + p[p.length-1];
        }

        if (b.stereotype == 'Section') {
            bname = b.id;
        }
        else if (b.stereotype == 'Header' || b.stereotype == 'Function') {
            var p = b.alias.split(/[\. ]/);
            bname = p[0];
            for(var i=1; i<p.length; i++){
                bname += '.';
                if (p[i]<10) bname += '0';
                bname += p[i];
            }
        }
        else if (b.stereotype == 'Criteria') {
            var p = b.name.split(/[\.#]/);
            bname = p[0];
            for(var i=1; i<p.length-1; i++){
                bname += '.';
                var num = Number(p[i]);
                if (num<10) bname += '0';
                bname += num;
            }
            bname += '#' + p[p.length-1];
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
