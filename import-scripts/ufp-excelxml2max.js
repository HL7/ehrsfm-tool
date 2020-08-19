var fs = require('fs');
var xml2js = require('xml2js');
 
var parser = new xml2js.Parser();

/*
 It was decided to make the Usability FP a companion FP, so there is no compilation step required.
 This script will convert the definition to a HL7-FM-Profile.
 */
var rawxmlfp = fs.readFileSync('EHRS_USEGUIDE_R1_I1_2019JAN_Usability_Functionlist_20190104_20200224b.xml');
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
        if (!STATEMENTcp) STATEMENT = "";
        var DESCRIPTION = getCellValue(row, 12);
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
                var parentID = ID.substring(0,ID.indexOf('.'));
                if(fmidx[parentID] == undefined) {
                    fmidx[parentID] = parentID;
                    obj['model'].objects.object.push({ 
                        id: parentID, 
                        name: "Section Placeholder",
                        alias: parentID,
                        stereotype: "Section", 
                        type: _type,
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
                    parentId: parentID,
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
                obj['model'].objects.object.push({ 
                    id: rowno, 
                    name: _name,
                    notes: CRITERIA,
                    stereotype: _stereotype, 
                    type: _type,
                    parentId: destId,
                    tag: [ { $: { name: "Row", value: rowno } },
                        { $: { name: "Optinality", value: '' } },
                        { $: { name: "Conditional", value: CRITERIA.startsWith("IF ")?"Y":"N" } },
                        { $: { name: "Dependent", value: "N" } },
                        { $: { name: "Reference.ChangeInfo", value: "N" } }
                    ]
                });
                break;
            default:
                break;
        }
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
