# Personal Health Record System Functional Model
cd /app/import-scripts
node xlsx2max.js convert -m true -t 2 -n PHR_R2 -i 'input/PHRSFM_R2_N1_2019JAN_import from template 20210526 jr 20210629.xlsx' > /app/publication/source/phrs-fm.max
cd /app/HL7_FM_CLI.ConsoleApp 
dotnet run validate /app/publication/source/phrs-fm.max
cd /app/publication
ant FM -Dfmname=phrs-fm