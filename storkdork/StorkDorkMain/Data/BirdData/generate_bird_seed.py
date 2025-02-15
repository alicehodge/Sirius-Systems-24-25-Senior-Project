import csv

# This script generates a SQL seed script to populate the Bird table in the database with the data from bird-taxonomy.csv

# bird-taxonomy.csv contains bird data sourced from the 
# Clements Checklist v2024 and the eBird/Clements Checklist v2024
# Downloaded from https://www.birds.cornell.edu/clementschecklist/download/

def clean_string(s):
    if s is None or s == '':
        return 'NULL'
    return f"'{s.replace("'", "''")}'"

def write_insert_batch(sqlfile, values):
    sqlfile.write("INSERT INTO [Bird] (\n")
    sqlfile.write("    [ScientificName],\n")
    sqlfile.write("    [CommonName],\n")
    sqlfile.write("    [SpeciesCode],\n")
    sqlfile.write("    [Category],\n")
    sqlfile.write("    [Order],\n")
    sqlfile.write("    [FamilyCommonName],\n")
    sqlfile.write("    [FamilyScientificName],\n")
    sqlfile.write("    [ReportAs],\n")
    sqlfile.write("    [Range]\n")
    sqlfile.write(")\nVALUES\n")
    sqlfile.write(',\n'.join(values))
    sqlfile.write(';\n\n')

def generate_sql():
    sql_file = '../DatabaseScripts/bird_seed.sql'
    csv_file = 'bird-taxonomy.csv'
    batch_size = 1000
    
    with open(csv_file, 'r', encoding='utf-8') as csvfile, \
         open(sql_file, 'w', encoding='utf-8') as sqlfile:
        
        # Write header
        sqlfile.write("-- Auto-generated Bird seed script\n\n")
        sqlfile.write("-- Clear existing data\n")
        sqlfile.write("DELETE FROM [Bird];\n\n")
        
        reader = csv.DictReader(csvfile)
        values = []
        batch_count = 0
        
        for row in reader:
            value_str = f"({clean_string(row['ScientificName'])}, "
            value_str += f"{clean_string(row['CommonName'])}, "
            value_str += f"{clean_string(row['SpeciesCode'])}, "
            value_str += f"{clean_string(row['Category'])}, "
            value_str += f"{clean_string(row['Order'])}, "
            value_str += f"{clean_string(row['FamilyCommonName'])}, "
            value_str += f"{clean_string(row['FamilyScientificName'])}, "
            value_str += f"{clean_string(row['ReportAs'])}, "
            value_str += f"{clean_string(row['Range'])})"
            values.append(value_str)
            
            # When we reach batch_size, write the batch and start a new one
            if len(values) >= batch_size:
                batch_count += 1
                sqlfile.write(f"-- Batch {batch_count}\n")
                write_insert_batch(sqlfile, values)
                values = []
        
        # Write any remaining values
        if values:
            batch_count += 1
            sqlfile.write(f"-- Batch {batch_count}\n")
            write_insert_batch(sqlfile, values)

if __name__ == '__main__':
    generate_sql()