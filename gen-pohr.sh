# HL7 EHR-System Problem-Oriented Health Record (POHR) Functional Profile, Release 1 (EHR_POHR_FP_R1_I1_2022JAN )
cd /app/import-scripts
node xlsx2max.js convert -n POHR -i input/POHR.xlsx > output/pohr-profiledefinition.max
cd /app/HL7_FM_CLI.ConsoleApp 
dotnet run compile /app/import-scripts/input/ehrs_fm_r2_1-2022APR.max /app/import-scripts/output/pohr-profiledefinition.max /app/publication/source/pohr.max
read -p "Remove TI and RI Section from source/pohr.max file and press enter to continue"
dotnet run validate /app/publication/source/pohr.max
cd /app/publication
ant FP -Dfpname=pohr

cd /app/publication/pdf
java -jar /opt/pdfbox-app-2.0.24.jar PDFMerger /app/import-scripts/input/EHR_WG-POHR_FP_Cover_Page-202112.pdf pohr.pdf pohr+coverpage.pdf
