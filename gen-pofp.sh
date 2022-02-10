# Podiatry Functional Profile
cd /app/import-scripts
node xlsx2max.js convert -t 1 -n POFP -i 'input/Post Ballot Podiatry Profile Working Spreadsheet MZ.xlsx' > output/pofp-profiledefinition.max
cd /app/HL7_FM_CLI.ConsoleApp 
dotnet run compile /app/import-scripts/input/ehrs_fm_r2_1-2020APR.max /app/import-scripts/output/pofp-profiledefinition.max /app/publication/source/pofp.max
dotnet run validate /app/publication/source/pofp.max
cd /app/publication
ant FP -Dfpname=pofp

cd /app/publication/pdf
java -jar /opt/pdfbox-app-2.0.24.jar PDFMerger /app/import-scripts/input/2021_Informative_CoverPage_Template_POFPErrata.pdf pofp.pdf pofp+coverpage.pdf