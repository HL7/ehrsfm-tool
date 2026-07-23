# 
cd /app/import-scripts
node xlsx2max.js convert -m true -n UFP -t 2 -i "input/EHR_UFP_Functionlist_20220606_Final+Draft_20220725.xlsx" > /app/publication/source/ufp.max
cd /app/HL7_FM_CLI.ConsoleApp 
#dotnet run compile /app/import-scripts/input/ehrs_fm_r2_1-2020APR.max /app/import-scripts/output/ufp-profiledefinition.max /app/publication/source/ufp.max
dotnet run validate /app/publication/source/ufp.max
echo Manually edit ufp.max, set stereotype to "HL7-FM-Profile" and press enter continue
read
cd /app/publication
ant FP -Dfpname=ufp

cd /app/publication/pdf
java -jar /opt/pdfbox-app-2.0.24.jar PDFMerger /app/import-scripts/input/2024_Normative_CoverPage_UFP_20240707.pdf ufp.pdf ufp+coverpage.pdf
