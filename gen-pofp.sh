# Podiatry Functional Profile
cd /app/import-scripts
node xlsx2max.js convert -t 1 -n POFP -i 'input/2021-05-18 Podiatry Profile Working Spreadsheet.xlsx' > output/pofp-profiledefinition.max
cd /app/HL7_FM_CLI.ConsoleApp 
dotnet run compile /app/import-scripts/input/ehrs_fm_r2_1-2020APR.max /app/import-scripts/output/pofp-profiledefinition.max /app/publication/source/pofp.max
dotnet run validate /app/publication/source/pofp.max
cd /app/publication
ant FP -Dfpname=pofp

# pdfunite ../../import-scripts/input/2021_Informative_CoverPage_Template_POFPErrata.pdf pofp.pdf pofp+coverpage.pdf