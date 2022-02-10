/*
 This script will convert a profile definition to a HL7-FM-Profile-Definition.max file
 that can then be used to compile into a compiled profile that is then input for publication.
 
 2021-apr-27; add yargs for command line options to make it a generic script 
 2021-apr-26; pchifp based on dhfp; don't include all ehr-s-fm functions
 */
var fs = require('fs');
const { abort } = require('process');
var xml2js = require('xml2js');
const readXlsxFile = require('read-excel-file/node');
var parser = new xml2js.Parser();
const { execSync } = require('child_process');
var revision = execSync('git rev-list --count HEAD');

// https://www.npmjs.com/package/yargs
var argv = require('yargs/yargs')(process.argv.slice(2))
.usage('Usage: $0 <command> [options]')
.command('convert', 'Convert a EHR-S Functional Model Profile spreadsheet to an EHR-S FM MAX file, input to the FPcompiler', {}, convert)
.option('sheet', { alias: 't', default: 2, nargs: 1, describe: 'The sheet number (tab) or name' })
.option('sort', { alias: 's', default: false, type: 'boolean', nargs: 1, describe: 'Sort based on FM chapters' })
.option('name', { alias: 'n', default: 'TSTFP', nargs: 1, describe: 'The short name of the FM/FP' })
.option('full', { alias: 'f', default: false, type: 'boolean', describe: 'Make this a full FP, include all from FM' })
.option('base', { alias: 'b', default: 'input/ehrs_fm_r2_1-2020APR.max', describe: 'The base FM to profile' })
.option('isfm', { alias: 'm', default: false, type: 'boolean', nargs: 1, describe: 'This is FP(false) or FM(true)' })
.option('input', { alias: 'i', nargs: 1, describe: 'The FP xlsx file name' })
.demandOption(['i'])
.demandCommand(1, 'You need at least one command before moving on')
.help('h')
.alias('h', 'help')
.version('1.0.' + revision)
.strict()
.argv;

function convert(args) {
    var PID = args.name;
    var obj = {
        'model': {
            $: { 'xmlns': 'http://www.umcg.nl/MAX' },
            objects: {
                $: { 'xmlns': '' },
                object: [ {
                    id: PID, 
                    name: PID,
                    alias: "",
                    stereotype: args.isfm?"HL7-FM":"HL7-FM-ProfileDefinition",
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

    var rawxmlfm = fs.readFileSync(args.base);
    var lookupfm = [];
    var lookupfmname = [];
    var lookupcriteria = [];
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

                    if(args.full) {
                        obj['model'].objects.object.push({ 
                            id: PID+ALIAS,
                            name: lookupfmname[ALIAS],
                            alias: ALIAS,
                            stereotype: "CI", 
                            type: "Class",
                            parentId: PID,
                            tag: [ { $: { name: "Reference.ChangeIndicator", value: 'NC' } } ]                    
                        });
                        obj['model'].relationships.relationship.push({
                            sourceId: PID+ALIAS,
                            destId: lookupfm[ALIAS], // lookup based on alias in base fm
                            notes: "",
                            type: "Generalization"
                        });
                    }                
                    break;
                case "Criteria":
                    lookupfm[object.name[0]] = object.id[0];
                    if (object.notes) {
                        lookupcriteria[object.name[0]] = object.notes[0].trim();
                    }
                    break;
            }
        });
    });

    // Map columns of the template 'Model' sheet to a js-struct
    const map = {
        'ID#': 'id',
        'Type': 'type',
        'Name': 'name',
        'Overview / Statement': 'statement',
        'Description': 'description',
        'Example(s)': 'example',
        'CC#': 'cc',
        'Conformance Criterium': 'criteria',
        'ChangeIndicator': 'changeindicator'
    }

    var cntS = 0;
    var cntH = 0;
    var cntF = 0;
    var cntC = 0;
    var cntNH = 0;
    var cntNF = 0;
    var cntNC = 0;

    // Remove first row that is nonce
    readXlsxFile(args.input, { sheet: args.sheet, map, transformData(data)
        { data.shift(); return data; } }).then(( {rows, errors}) => {
        // `rows` is an array of rows
        // each row being an array of cells.
        //console.error(JSON.stringify(errors, null, 2));
        var rowno = 2
        var secno = 1;
        rows.forEach(row => { 
            rowno++;
            var ID = row.id;
            var TYPE = row.type;
            var NAME = row.name;
            var STATEMENT = row.statement?row.statement:"";
            var DESCRIPTION = row.description?row.description:"";
            var EXAMPLE = row.example?row.example:"";
            var CC = row.cc?row.cc:undefined;
            var CRITERIA = row.criteria?row.criteria:"";
            var FLAG = row.changeindicator;
            //console.error(`${ID} ${TYPE} ${NAME} ${STATEMENT} ${CC} ${CRITERIA} ${FLAG}`);

            switch (TYPE) {
                case 'T':
                    var object = obj['model'].objects.object.find(element => element.alias == ID);
                    if (object == undefined) {
                        // if this is a new section!
                        if (lookupfm[ID] == undefined) {
                            object = { 
                                id: PID + rowno, 
                                name: NAME,
                                alias: ID,
                                notes: `$OV$${STATEMENT}${DESCRIPTION}$EX$${EXAMPLE}$AC$`,
                                stereotype: "Section", 
                                type: "Package",
                                parentId: PID,
                                tag: [ { $: { name: "ID", value: secno++ } } ]
                            };
                            // for new F/H on this Section
                            lookupfm[ID] = PID + rowno;
                            obj['model'].objects.object.push(object);
                            cntS++;
                        }
                        // else TODO: Lookup section and add generalization? Is it allowed to redefine a Section?
                    }
                    break;            
                case 'H':
                case 'F':
                    // first lookup already created object
                    var object = obj['model'].objects.object.find(element => element.alias == ID);
                    if (object == undefined) {
                        // if this is a new header or function!
                        if (args.isfm || lookupfm[ID] == undefined) {
                            // Aggregation to parent
                            var parentID = ID.substring(0, ID.lastIndexOf('.'));
                            object = { 
                                id: PID + rowno, 
                                name: ID + " " + NAME,
                                alias: ID,
                                notes: `$ST$${STATEMENT}$DE$${DESCRIPTION}$EX$${EXAMPLE}`,
                                stereotype: (TYPE=='H'?"Header":"Function"), 
                                type: "Feature",
                                parentId: lookupfm[parentID], // lookup based on alias in base fm,
                            };
                            // for new Criteria on this Function
                            lookupfm[ID] = PID + rowno;
                            obj['model'].relationships.relationship.push({
                                sourceId: PID + rowno,
                                destId: lookupfm[parentID], // lookup based on alias in base fm
                                notes: "",
                                type: "Aggregation"
                            });
                            if (TYPE=='H') cntNH++; else cntNF++;
                        }
                        else {
                            object = { 
                                id: PID + rowno, 
                                name: lookupfmname[ID],
                                alias: ID,
                                stereotype: "CI", 
                                type: "Class",
                                parentId: PID,
                            };
                            if (NAME) {
                                object.name = `${ID} ${NAME}`;
                            }
                            obj['model'].relationships.relationship.push({
                                sourceId: PID + rowno,
                                destId: lookupfm[ID], // lookup based on alias in base fm
                                notes: "",
                                type: "Generalization"
                            });
                            // update only if not both empty or this is a model, then it is just an include in the FP
                            if (STATEMENT != "" && DESCRIPTION != "") {
                                object.notes = `$ST$${STATEMENT}$DE$${DESCRIPTION}$EX$${EXAMPLE}`;
                            }
                            if (TYPE=='H') cntH++; else cntF++;
                        }
                        obj['model'].objects.object.push(object);
                    }
                    object.tag = [ { $: { name: "Row", value: rowno } },
                            { $: { name: "Reference.ChangeIndicator", value: FLAG } } ];
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
                        CRITERIA.includes('jurisdictional law')) _dependent = "Y";

                    if (args.isfm || lookupfm[_name] == undefined) {
                        // this is a new Criteria
                        obj['model'].objects.object.push({ 
                            id: PID + rowno, 
                            name: _name,
                            notes: CRITERIA,
                            stereotype: "Criteria", 
                            type: "Requirement",
                            parentId: lookupfm[ID],
                            tag: [ { $: { name: "Row", value: rowno } },
                                { $: { name: "Optionality", value: _optionality } },
                                { $: { name: "Conditional", value: CRITERIA.startsWith("IF ")?"Y":"N" } },
                                { $: { name: "Dependent", value: _dependent } },
                                { $: { name: "Reference.ChangeIndicator", value: FLAG } }
                            ]
                        });
                        // Aggregation new Criteria to parent Function
                        obj['model'].relationships.relationship.push({
                            sourceId: PID + rowno,
                            destId: lookupfm[ID],
                            notes: "",
                            type: "Aggregation"
                        });
                        cntNC++;
                    }
                    else {
                        var object = { 
                            id: PID + rowno, 
                            name: _name,
                            stereotype: "CI", 
                            type: "Class",
                            parentId: PID,
                            tag: [ { $: { name: "Row", value: rowno } },
                                { $: { name: "Optionality", value: _optionality } },
                                { $: { name: "Conditional", value: CRITERIA.startsWith("IF ")?"Y":"N" } },
                                { $: { name: "Dependent", value: _dependent } },
                                { $: { name: "Reference.ChangeIndicator", value: FLAG } }
                            ]
                        };
                        obj['model'].objects.object.push(object);
                        if (lookupcriteria[_name] != CRITERIA) {
                            object.notes = CRITERIA;
                        }

                        obj['model'].relationships.relationship.push({
                            sourceId: PID + rowno,
                            destId: lookupfm[_name], // lookup based on _name in base fm
                            notes: "",
                            type: "Generalization"
                        });
                        cntC++;
                    }
                    break;
                default:
                    break;
            }
        });

        // Optionally sort
        if(args.sort) {
            sort(obj);
        }

        // Statistics
        var cntO = obj['model'].objects.object.length;
        // var cntS = obj['model'].objects.object.filter(obj => obj.stereotype == "Section").length;
        // var cntH = obj['model'].objects.object.filter(obj => obj.stereotype == "Header").length;
        // var cntF = obj['model'].objects.object.filter(obj => obj.stereotype == "Function").length;
        // var cntC = obj['model'].objects.object.filter(obj => obj.stereotype == "Criteria").length;
        var cntCI = obj['model'].objects.object.filter(obj => obj.stereotype == "CI").length;
        console.error ('');
        console.error ('STATISTICS - total (existing/new)');
        console.error ('----------');
        console.error (`Objects  : ${cntO}`);
        console.error (`Sections : ${cntS}`);
        console.error (`Headers  : ${cntH+cntNH} (${cntH}/${cntNH})`);
        console.error (`Functions: ${cntF+cntNF} (${cntF}/${cntNF})`);
        console.error (`Criteria : ${cntC+cntNC} (${cntC}/${cntNC})`);
        console.error (`CI       : ${cntCI}`);
        console.error ('');

        // Dump max xml 
        var builder = new xml2js.Builder();
        console.log (builder.buildObject(obj))
    });
}

var section_sortkey = [];
// EHR-S FM and UFP chapters
section_sortkey['OV'] = '1OV';
section_sortkey['CP'] = '2CP';
section_sortkey['CPS'] = '3CPS';
section_sortkey['AS'] = '4AS';
section_sortkey['POP'] = '5POP';
section_sortkey['RI'] = '6RI';
section_sortkey['TI'] = '7TI';
section_sortkey['U'] = '8U';    
// PHR-S FM chapters
var section_sortkey = [];
section_sortkey['PH'] = '1PH';
section_sortkey['S'] = '2S';
section_sortkey['RI'] = '3RI';
section_sortkey['TI'] = '4TI';

function sort(obj) {
    // sort by FM ID
    obj['model'].objects.object.sort(function (a,b) {
        var aname = a.name;
        var bname = b.name;
        // Package should be first!
        if (a.stereotype == 'HL7-FM-ProfileDefinition' || a.stereotype == 'HL7-FM-Profile' || a.stereotype == 'HL7-FM') {
            aname = "0" + aname;
        }
        if (a.stereotype == 'HL7-FM-ProfileDefinition' || b.stereotype == 'HL7-FM-Profile' || a.stereotype == 'HL7-FM') {
            bname = "0" + bname;
        }

        if (a.stereotype == 'Section') {
            aname = section_sortkey[a.alias];
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
            bname = section_sortkey[b.alias];
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
}

function formatID(ID, CC) {
    var cc = `${CC}`;
    if (cc.length < 2) cc = `0${cc}`;
    return `${ID}#${cc}`;
}
