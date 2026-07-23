# MedList FP; just compile publication is IG publishing
cd /app/HL7_FM_CLI.ConsoleApp 
dotnet run compile /app/publication/source/phrs-fm.max /app/import-scripts/output/medlist-profiledefinition.max /app/import-scripts/output/medlist.max
dotnet run validate /app/import-scripts/output/medlist.max
