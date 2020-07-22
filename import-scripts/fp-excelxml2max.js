var fs = require('fs');
var xml2js = require('xml2js');
const { Console } = require('console');
 
var parser = new xml2js.Parser();

// read base FM
var rawxmlfm = fs.readFileSync('/home/michael/Shared/VisualStudio Projects/ehrsfm_profile - SVN OLD VERSION/publication/source/ehrs-fm.max');
var jsfm;
parser.parseString(rawxmlfm, function (err, result) {
    jsfm = result;
});
// build fm index
var fmidx = [];
jsfm.model.objects.forEach(objects => {
    objects.object.forEach(object => {
        switch(`${object.stereotype}`) {
            case "Section":
            case "Header":
            case "Function":
                fmidx[`${object.alias}`] = object.id;
                break;
            case "Criteria":
                fmidx[`${object.name}`] = object.id;
                break;
        }
    });
});

// convert fp definition spreadsheet
var rawxmlfp = fs.readFileSync('EHRS_USEGUIDE_R1_I1_2019JAN_Usability_Functionlist_20190104_20200224b.xml');
parser.parseString(rawxmlfp, function (err, result) {
    var PID = "UFP";
    var obj = {
        'max:model': {
            $: { 'xmlns:max': 'http://www.umcg.nl/MAX' },
            objects: [ {
                object: {
                    id: PID, 
                    name: "UFP",
                    alias: "",
                    stereotype: "HL7-FM-ProfileDefinition", 
                    type: "Package",
                    tag: [ { $: { name: "PrioritiesDescription", value: "" } },
                        { $: { name: "Type", value: "Domain" } },
                        { $: { name: "Rationale", value: "<memo>" } },
                        { $: { name: "Scope", value: "<memo>" } },
                        { $: { name: "LanguageTag", value: "" } },
                        { $: { name: "PrioritiesDefinition", value: "<memo>" } },
                        { $: { name: "ConformanceClause", value: "<memo>" } } 
                    ]
                }
            } ],
            relationships: []
        }
    };

    var sheet = result.Workbook.Worksheet.find(sheet => sheet.$['ss:Name'] === "Usability_FP");
    var lastID = undefined;
    var rowno = 0
    sheet.Table[0].Row.forEach(row => {
        rowno++;
        var ID = getCellValue(row, 6);
        var TYPE = getCellValue(row, 7);
        var NAME = getCellValue(row, 8);
        var CC = getCellValue(row, 14);
        var CRITERIA = getCellValue(row, 15);
        var FLAG = getCellValue(row, 24);
        if (!ID) {
            ID = lastID;
        }
        else {
            lastID = ID;
        }
        //console.log(`${ID} ${TYPE} ${NAME} ${CC} ${CRITERIA} ${FLAG}`);

        switch (TYPE) {
            case 'F':
                var _stereotype = "Function";
                var _type = "Feature";
                if (FLAG === 'C') {
                    _stereotype = "CI";
                    _type = "Class";
                }
                obj['max:model'].objects.push({ 
                    object: {
                        id: rowno, 
                        name: `${ID} ${NAME}`,
                        alias: ID,
                        stereotype: _stereotype, 
                        type: _type,
                        parentId: PID,
                        tag: [ { $: { name: "Row", value: rowno } },
                            { $: { name: "Reference.ChangeInfo", value: FLAG } } ]
                    } 
                });
                fmidx[`${ID}`] = rowno;
                break;
            case 'C':
                if (!CRITERIA) {
                    CRITERIA = JSON.stringify(row.Cell[15]['ss:Data'][0]);
                }
                var _name = formatID(ID,CC);
                var _stereotype = "Criteria";
                var _type = "Requirement";
                if (FLAG === 'C') {
                    var destId = fmidx[_name];
                    if (!destId) {
                        // destId not found, assume NEW
                        FLAG = "N";
                        console.error(`destId not found for ${_name} assume NEW`);

                        destId = fmidx[`${ID}`];
                        if (!destId) {
                            console.error(`parent not found for ${ID}`);
                        }
                        // add Aggregation
                        obj['max:model'].relationships.push({
                            relationship: {
                                sourceId: rowno,
                                destId: destId,
                                type: "Aggregation"
                            }
                        });
                    }
                    else {
                        _stereotype = "CI";
                        _type = "Class";
                        // add Generalization
                        obj['max:model'].relationships.push({
                            relationship: {
                                sourceId: rowno,
                                destId: destId,
                                type: "Generalization"
                            }
                        });
                    }
                }
                else {
                    // TODO: add Aggregation to parent Header/Function
                }
                obj['max:model'].objects.push({ 
                    object: {
                        id: rowno, 
                        name: _name,
                        notes: CRITERIA,
                        stereotype: _stereotype, 
                        type: _type,
                        parentId: PID,
                        tag: [ { $: { name: "Row", value: rowno } },
                            { $: { name: "Optinality", value: '' } },
                            { $: { name: "Conditional", value: CRITERIA.startsWith("IF ")?"Y":"N" } },
                            { $: { name: "Dependent", value: "N" } },
                            { $: { name: "Reference.ChangeInfo", value: FLAG } }
                        ]
                    } 
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