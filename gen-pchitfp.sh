# 
cd /app/import-scripts
node xlsx2max.js convert -n PCHITFP -t 1 -i input/PCHIT_postBallot_2022Jan11__Apr07.xlsx > output/pchitfp-profiledefinition.max
cd /app/HL7_FM_CLI.ConsoleApp 
dotnet run compile /app/import-scripts/input/ehrs_fm_r2_1-2020APR.max /app/import-scripts/output/pchitfp-profiledefinition.max /app/publication/source/pchitfp.max
dotnet run validate /app/publication/source/pchitfp.max
cd /app/publication
ant FP -Dfpname=pchitfp

cd /app/publication/pdf
java -jar /opt/pdfbox-app-2.0.24.jar PDFMerger /app/import-scripts/input/pchitfp_coverpage_apr2022.pdf pchitfp.pdf pchitfp+coverpage.pdf
