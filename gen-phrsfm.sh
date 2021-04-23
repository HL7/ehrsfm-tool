cd /app/import-scripts
node phr-xls2max.js input/PHRSFM_R2_N1_2019JAN_Functionlist_20181203_20190123_20201027_20210319_20210414.xlsx > /app/publication/source/phrs-fm.max
cd /app/publication
ant 3PHRSFM
