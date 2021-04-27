
rows = function(argv) {
  // Add to Dockerfile: npm install read-excel-file

  // https://www.npmjs.com/package/read-excel-file
  const readXlsxFile = require('read-excel-file/node');

  // Map columns of the template 'Model' sheet to a js-struct
  const dhfp_sheet = 2;
  const dhfp_ap = {
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

  readXlsxFile(argv.file, { sheet: argv.sheet, transformData(data)
      { data.shift(2); return data; }}).then((rows) => {
    // `rows` is an array of rows
    // each row being an array of cells.
    rows.forEach(row => {
      row.row = row[0];
      row.id = row[1]; 
      console.log(row);
      });
  })

  // readXlsxFile(process.argv[2], { sheet: 2, dhfp_map, transformData(data)
  //       { data.shift(2); return data; }}).then((rows) => {
  //   // `rows` is an array of rows
  //   // each row being an array of cells.
  //   rows.forEach(row => { 
  //     console.log(row);
  //     });
  // })
}

// Add to Dockerfile: npm install yargs

// https://www.npmjs.com/package/yargs
var argv = require('yargs/yargs')(process.argv.slice(2))
.usage('Usage: $0 <command> [options]')
.command('rows', 'Show rows of first tab', {}, rows)
.option('sheet', { alias: 's', default: 1, nargs: 1, describe: 'The sheet number or name' })
.option('file', { alias: 'f', nargs: 1, describe: 'The xlsx file name' })
.demandOption(['f'])
.demandCommand(1, 'You need at least one command before moving on')
.help('h')
.alias('h', 'help')
.strict()
.argv;
return;