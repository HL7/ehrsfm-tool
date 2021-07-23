# 
cd /app/import-scripts
node xlsx2max.js convert -n PCHITFP -t 1 -i input/PCHIT_20210607_20210719_DGRemoved.xlsx > output/pchitfp-profiledefinition.max
cd /app/HL7_FM_CLI.ConsoleApp 
dotnet run compile /app/import-scripts/input/ehrs_fm_r2_1-2020APR.max /app/import-scripts/output/pchitfp-profiledefinition.max /app/publication/source/pchitfp.max
dotnet run validate /app/publication/source/pchitfp.max
cd /app/publication
ant FP -Dfpname=pchitfp
pdfunite ../import-scripts/input/PCHIT_R1_FP_CoverPage_v1.0.pdf pdf/pchitfp.pdf pdf/pchitfp+coverpage.pdf