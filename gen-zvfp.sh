# ZorgViewer Functional Profile
cd /app/import-scripts
node xlsx2max.js convert -n ZVFP -i input/ZVFP.xlsx > output/zvfp-profiledefinition.max
cd /app/HL7_FM_CLI.ConsoleApp 
dotnet run compile /app/import-scripts/input/ehrs_fm_r2_1-2020APR.max /app/import-scripts/output/zvfp-profiledefinition.max /app/publication/source/zvfp.max
dotnet run validate /app/publication/source/zvfp.max
cd /app/publication
ant FP -Dfpname=zvfp