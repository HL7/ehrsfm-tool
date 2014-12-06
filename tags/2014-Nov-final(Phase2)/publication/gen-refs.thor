require 'csv'

class GenExternRef < Thor
  
  desc 'from_csv CSV-FILE', 'Create the Functional Profile external references from a converted spreadsheet'
  def from_csv(csv_file)
    CSV.foreach(csv_file, headers: true, col_sep: '|') do |row| 
      if row.length > 0
        puts "#{row[0]}<tag name=\"ExternalReference\" value=\"TEXT$$#{row[1]} #{row[2]}$$URL$$\"/>"
        puts "#{row[0]}<tag name=\"ExternalReference\" value=\"TEXT$$#{row[3]}$$URL$$#{row[4]}\"/>"
      end
    end
  end
end