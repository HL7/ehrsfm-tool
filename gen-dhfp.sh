cd /app/import-scripts
node xlsx2max.js convert -f -n DHFP -i input/DHFP+01122021+01222021+03192021+03252021+04062021.xlsx > output/dhfp-profiledefinition.max
cd /app/HL7_FM_CLI.ConsoleApp 
dotnet run compile /app/import-scripts/input/ehrs_fm_r2_1-2020APR.max /app/import-scripts/output/dhfp-profiledefinition.max /app/publication/source/dhfp.max
dotnet run validate /app/publication/source/dhfp.max
cd /app/publication
ant FP -Dfpname=dhfp
