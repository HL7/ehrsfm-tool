cd /app/import-scripts
node xlsx2max.js convert -m true -n CMHAFF -i 'input/CMHAFF migrate to EHR-S FM format 20210616.xlsx' > /app/publication/source/cmhaff.max
cd /app/HL7_FM_CLI.ConsoleApp 
dotnet run validate /app/publication/source/cmhaff.max
cd /app/publication
ant FM -Dfmname=cmhaff
