cd /app/import-scripts
node xlsx2max.js convert -f -n PCHIFP -i input/PCHIFP_20210426.xlsx > output/pchifp-profiledefinition.max
cd /app/HL7_FM_CLI.ConsoleApp 
dotnet run compile /app/import-scripts/input/ehrs_fm_r2_1-2020APR.max /app/import-scripts/output/pchifp-profiledefinition.max /app/publication/source/pchifp.max
dotnet run validate /app/publication/source/pchifp.max
cd /app/publication
ant PCHIFP
