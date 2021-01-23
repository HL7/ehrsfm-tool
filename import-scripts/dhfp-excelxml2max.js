var fs = require('fs');
const { abort } = require('process');
var xml2js = require('xml2js');
 
var parser = new xml2js.Parser();

/*
 This script will convert the profile definition to a HL7-FM-Profile..
 that can then be used to compile into a compiled profile that is then input for publication.
 The input is a xml export of an excel XML mapped to fp-schema.xml.

 2020-dec-20; sort Profile/Section(s)/Header/Function/Criteria by ID en seq#
 2020-dec-22; order of Sections fixed
 */
var rawxmlfm = fs.readFileSync('input/ehrs_fm_r2_1-2020APR.max');
var lookupfm = [];
parser.parseString(rawxmlfm, function (err, result) {
    result['model'].objects[0].object.forEach(object => {
        lookupfm[object.name[0]] = object.id[0];
    });
});


var rawxmlfp = fs.readFileSync('input/DHFP GZ 1-12-2021 MZ 1-22-2021.xml');
parser.parseString(rawxmlfp, function (err, result) {
    var PID = "DHFP";
    var obj = {
        'model': {
            $: { 'xmlns': 'http://www.umcg.nl/MAX' },
            objects: {
                $: { 'xmlns': '' },
                object: [ {
                    id: PID, 
                    name: "DHFP",
                    alias: "",
                    stereotype: "HL7-FM-ProfileDefinition",
                    type: "Package",
                    tag: [ { $: { name: "PrioritiesDescription", value: "" } },
                        { $: { name: "Type", value: "Domain" } },
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

    var rowno = 0
    result.fp.row.forEach(row => {
        rowno++;
        var ID = row.id[0];
        var TYPE = row.type[0];
        var NAME = row.name?row.name[0]:undefined;
        var STATEMENT = row.statement?row.statement[0]:undefined;
        if (!STATEMENT) STATEMENT = "";
        var DESCRIPTION = row.description?row.description[0]:undefined;
        if (!DESCRIPTION) DESCRIPTION = "";
        var CC = row.cc?row.cc[0]:undefined;
        var CRITERIA = row.criteria?row.criteria[0]:undefined;
        var FLAG = row.changeindicator[0];
        // console.log(`${ID} ${TYPE} ${NAME} ${CC} ${CRITERIA} ${FLAG}`);

        switch (TYPE) {
            case 'H':
            case 'F':
                var _stereotype = "CI";
                var _type = "Class";
                obj['model'].objects.object.push({ 
                    id: rowno, 
                    name: `${ID} ${NAME}`,
                    alias: ID,
                    notes: `$ST$${STATEMENT}$DE$${DESCRIPTION}$EX$`,
                    stereotype: _stereotype, 
                    type: _type,
                    parentId: PID,
                    tag: [ { $: { name: "Row", value: rowno } },
                        { $: { name: "Reference.ChangeInfo", value: FLAG } } ]
                });
                break;
            case 'C':
                var _name = formatID(ID, CC);
                var _stereotype = "CI";
                var _type = "Class";
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
                    parentId: PID,
                    tag: [ { $: { name: "Row", value: rowno } },
                        { $: { name: "Optionality", value: _optionality } },
                        { $: { name: "Conditional", value: CRITERIA.startsWith("IF ")?"Y":"N" } },
                        { $: { name: "Dependent", value: _dependent } },
                        { $: { name: "Reference.ChangeInfo", value: FLAG } }
                    ]
                });
                obj['model'].relationships.relationship.push({
                    label: undefined,
                    sourceId: rowno,
                    sourceLabel: undefined,
                    sourceCard: undefined,
                    destId: lookupfm[_name], // lookup based on _name in base fm
                    destLabel: undefined,
                    destCard: undefined,
                    notes: "",
                    stereotype: undefined,
                    type: "Generalization"
                });
                break;
            default:
                break;
        }
    });

    var builder = new xml2js.Builder();
    console.log (builder.buildObject(obj))
});

function formatID(ID, CC) {
    var cc = `${CC}`;
    if (cc.length < 2) cc = `0${cc}`;
    return `${ID}#${cc}`;
}
