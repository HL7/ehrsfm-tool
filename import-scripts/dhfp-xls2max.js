/*
 This script will convert the profile definition to a HL7-FM-Profile..
 that can then be used to compile into a compiled profile that is then input for publication.
 The input is a xml export of an excel XML mapped to fp-schema.xml.

 2021-apr-22; read xlsx directly
 2021-mar-26; decided by WG to make this a "full" "domain" FM FP <- this is not currently a official type of profile
 2021-jan-27; added Generalization relationship to base for Header/Function
              assume name of Header/Function cannot be changed so lookup in base
 2021-jan-12; added Generalization relationship to base for Criteria
 2020-dec-20; sort Profile/Section(s)/Header/Function/Criteria by ID en seq#
 2020-dec-22; order of Sections fixed
 */
var fs = require('fs');
const { abort } = require('process');
var xml2js = require('xml2js');
const readXlsxFile = require('read-excel-file/node');
var parser = new xml2js.Parser();

if (process.argv.length <= 2) {
    console.error("First argument should be the DHFP xlsx filename.")
    return;
}

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

var rawxmlfm = fs.readFileSync('input/ehrs_fm_r2_1-2020APR.max');
var lookupfm = [];
var lookupfmname = [];
parser.parseString(rawxmlfm, function (err, result) {
    result['model'].objects[0].object.forEach(object => {
        switch (object.stereotype[0]) {
            case "Header":
            case "Function":
                var ALIAS = object.alias[0];
                lookupfm[ALIAS] = object.id[0];
                lookupfmname[ALIAS] = object.name[0];

                if (ALIAS == '') {
                    return;
                }

                obj['model'].objects.object.push({ 
                    id: PID+ALIAS,
                    name: lookupfmname[ALIAS],
                    alias: ALIAS,
                    stereotype: "CI", 
                    type: "Class",
                    parentId: PID,
                    tag: [ { $: { name: "Reference.ChangeInfo", value: 'NC' } } ]                    
                });
                obj['model'].relationships.relationship.push({
                    sourceId: PID+ALIAS,
                    destId: lookupfm[ALIAS], // lookup based on alias in base fm
                    notes: "",
                    type: "Generalization"
                });
                break;
            case "Criteria":
                lookupfm[object.name[0]] = object.id[0];
                break;
        }
    });
});

// Map columns of the template 'Model' sheet to a js-struct
const map = {
    'row#': 'row',
    'ID#': 'id',
    'Type': 'type',
    'Name': 'name',
    'Overview / Statement': 'statement',
    'Description': 'description',
    'CC#': 'cc',
    'Conformance Criterium': 'criteria',
    'ChangeIndicator': 'changeindicator'
  }

// Remove first row that is nonce
readXlsxFile(process.argv[2], { sheet: 2, map, transformData(data)
    { data.shift(); return data; } }).then(( {rows, errors}) => {
    // `rows` is an array of rows
    // each row being an array of cells.
    // console.log(errors);
    var rowno = 0
    rows.forEach(row => { 
        rowno++;
        var ID = row.id;
        var TYPE = row.type;
        var NAME = row.name;
        var STATEMENT = row.statement?row.statement:"";
        var DESCRIPTION = row.description?row.description:"";
        var CC = row.cc?row.cc:undefined;
        var CRITERIA = row.criteria?row.criteria:"";
        var FLAG = row.changeindicator;
        //console.log(`${ID} ${TYPE} ${NAME} ${CC} ${CRITERIA} ${FLAG}`);

        switch (TYPE) {
            case 'H':
            case 'F':
                // first lookup already created object
                var object = obj['model'].objects.object.find(element => element.alias == ID);
                if (object == undefined) {
                    console.error("NOT EXISTING? " + ID);
                    object = { 
                        id: rowno, 
                        name: lookupfmname[ID],
                        alias: ID,
                        stereotype: "CI", 
                        type: "Class",
                        parentId: PID,
                    };
                    obj['model'].objects.object.push(object);
                    obj['model'].relationships.relationship.push({
                        label: undefined,
                        sourceId: rowno,
                        sourceLabel: undefined,
                        sourceCard: undefined,
                        destId: lookupfm[ID], // lookup based on alias in base fm
                        destLabel: undefined,
                        destCard: undefined,
                        notes: "",
                        stereotype: undefined,
                        type: "Generalization"
                    });                   
                }
                // update
                object.notes = `$ST$${STATEMENT}$DE$${DESCRIPTION}$EX$`;
                object.tag = [ { $: { name: "Row", value: rowno } },
                        { $: { name: "Reference.ChangeInfo", value: FLAG } } ];
                break;
            case 'C':
                var _name = formatID(ID, CC);
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
                    stereotype: "CI", 
                    type: "Class",
                    parentId: PID,
                    tag: [ { $: { name: "Row", value: rowno } },
                        { $: { name: "Optionality", value: _optionality } },
                        { $: { name: "Conditional", value: CRITERIA.startsWith("IF ")?"Y":"N" } },
                        { $: { name: "Dependent", value: _dependent } },
                        { $: { name: "Reference.ChangeInfo", value: FLAG } }
                    ]
                });
                obj['model'].relationships.relationship.push({
                    sourceId: rowno,
                    destId: lookupfm[_name], // lookup based on _name in base fm
                    notes: "",
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