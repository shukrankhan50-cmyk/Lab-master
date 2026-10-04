USE LabMasterDb;
GO
/* Lab Master - Broad laboratory test catalog
   Prices intentionally start at 0 so the laboratory can set local charges.
   Run after 004, 005 and 010. Existing TestCode values are preserved. */
IF NOT EXISTS(SELECT 1 FROM dbo.Departments WHERE DepartmentName=N'Special Chemistry') INSERT dbo.Departments(DepartmentName) VALUES(N'Special Chemistry');
IF NOT EXISTS(SELECT 1 FROM dbo.Departments WHERE DepartmentName=N'Immunology') INSERT dbo.Departments(DepartmentName) VALUES(N'Immunology');
GO

DECLARE @H INT=(SELECT DepartmentId FROM dbo.Departments WHERE DepartmentName=N'Hematology');
DECLARE @B INT=(SELECT DepartmentId FROM dbo.Departments WHERE DepartmentName=N'Biochemistry');
DECLARE @S INT=(SELECT DepartmentId FROM dbo.Departments WHERE DepartmentName=N'Serology / Immunology');
DECLARE @I INT=(SELECT DepartmentId FROM dbo.Departments WHERE DepartmentName=N'Immunology');
DECLARE @SC INT=(SELECT DepartmentId FROM dbo.Departments WHERE DepartmentName=N'Special Chemistry');
DECLARE @M INT=(SELECT DepartmentId FROM dbo.Departments WHERE DepartmentName=N'Molecular / PCR');
DECLARE @CP INT=(SELECT DepartmentId FROM dbo.Departments WHERE DepartmentName=N'Clinical Pathology');
DECLARE @MB INT=(SELECT DepartmentId FROM dbo.Departments WHERE DepartmentName=N'Microbiology');
DECLARE @HP INT=(SELECT DepartmentId FROM dbo.Departments WHERE DepartmentName=N'Histopathology');

DECLARE @T TABLE(Code NVARCHAR(30),Name NVARCHAR(150),Dept INT,Sample NVARCHAR(80),Unit NVARCHAR(50));
INSERT @T VALUES
-- Hematology
(N'CBC',N'Complete Blood Count',@H,N'EDTA Blood',NULL),(N'ESR',N'Erythrocyte Sedimentation Rate',@H,N'EDTA Blood',N'mm/hr'),
(N'HB',N'Hemoglobin',@H,N'EDTA Blood',N'g/dL'),(N'RBC',N'Red Blood Cell Count',@H,N'EDTA Blood',N'x10^12/L'),
(N'WBC',N'White Blood Cell Count',@H,N'EDTA Blood',N'x10^9/L'),(N'PLT',N'Platelet Count',@H,N'EDTA Blood',N'x10^9/L'),
(N'PCV',N'Packed Cell Volume / Hematocrit',@H,N'EDTA Blood',N'%'),(N'MCV',N'Mean Corpuscular Volume',@H,N'EDTA Blood',N'fL'),
(N'MCH',N'Mean Corpuscular Hemoglobin',@H,N'EDTA Blood',N'pg'),(N'MCHC',N'Mean Corpuscular Hemoglobin Concentration',@H,N'EDTA Blood',N'g/dL'),
(N'RDW',N'Red Cell Distribution Width',@H,N'EDTA Blood',N'%'),(N'RETIC',N'Reticulocyte Count',@H,N'EDTA Blood',N'%'),
(N'BT',N'Bleeding Time',@H,N'Whole Blood',N'min'),(N'CT',N'Clotting Time',@H,N'Whole Blood',N'min'),
(N'PT-INR',N'Prothrombin Time / INR',@H,N'Citrated Plasma',NULL),(N'APTT',N'Activated Partial Thromboplastin Time',@H,N'Citrated Plasma',N'sec'),
(N'FIBRINOGEN',N'Fibrinogen',@H,N'Citrated Plasma',N'mg/dL'),(N'D-DIMER',N'D-Dimer',@H,N'Citrated Plasma',N'mg/L FEU'),
(N'BLOOD-FILM',N'Peripheral Blood Film',@H,N'EDTA Blood',NULL),(N'MALARIA-FILM',N'Malaria Parasite Film',@H,N'EDTA Blood',NULL),
-- Biochemistry
(N'GLU-F',N'Fasting Blood Glucose',@B,N'Fluoride Plasma',N'mg/dL'),(N'GLU-R',N'Random Blood Glucose',@B,N'Fluoride Plasma',N'mg/dL'),
(N'GLU-PP',N'Postprandial Blood Glucose',@B,N'Fluoride Plasma',N'mg/dL'),(N'HBA1C',N'HbA1c',@B,N'EDTA Blood',N'%'),
(N'UREA',N'Blood Urea',@B,N'Serum',N'mg/dL'),(N'CREAT',N'Creatinine',@B,N'Serum',N'mg/dL'),
(N'URIC',N'Uric Acid',@B,N'Serum',N'mg/dL'),(N'BILT',N'Total Bilirubin',@B,N'Serum',N'mg/dL'),
(N'BILD',N'Direct Bilirubin',@B,N'Serum',N'mg/dL'),(N'ALT',N'ALT / SGPT',@B,N'Serum',N'U/L'),
(N'AST',N'AST / SGOT',@B,N'Serum',N'U/L'),(N'ALP',N'Alkaline Phosphatase',@B,N'Serum',N'U/L'),
(N'GGT',N'Gamma GT',@B,N'Serum',N'U/L'),(N'TP',N'Total Protein',@B,N'Serum',N'g/dL'),
(N'ALB',N'Albumin',@B,N'Serum',N'g/dL'),(N'GLOB',N'Globulin',@B,N'Serum',N'g/dL'),
(N'CHOL',N'Total Cholesterol',@B,N'Serum',N'mg/dL'),(N'TG',N'Triglycerides',@B,N'Serum',N'mg/dL'),
(N'HDL',N'HDL Cholesterol',@B,N'Serum',N'mg/dL'),(N'LDL',N'LDL Cholesterol',@B,N'Serum',N'mg/dL'),
(N'VLDL',N'VLDL Cholesterol',@B,N'Serum',N'mg/dL'),(N'NA',N'Sodium',@B,N'Serum',N'mmol/L'),
(N'K',N'Potassium',@B,N'Serum',N'mmol/L'),(N'CL',N'Chloride',@B,N'Serum',N'mmol/L'),
(N'CA',N'Calcium',@B,N'Serum',N'mg/dL'),(N'PHOS',N'Phosphorus',@B,N'Serum',N'mg/dL'),
(N'MG',N'Magnesium',@B,N'Serum',N'mg/dL'),(N'AMYLASE',N'Amylase',@B,N'Serum',N'U/L'),
(N'LDH',N'Lactate Dehydrogenase',@B,N'Serum',N'U/L'),(N'CK',N'Creatine Kinase',@B,N'Serum',N'U/L'),
(N'CKMB',N'CK-MB',@B,N'Serum',N'U/L'),(N'CRP-Q',N'Quantitative CRP',@B,N'Serum',N'mg/L'),
-- Serology / ICT
(N'HBSAG-ICT',N'HBsAg - ICT',@S,N'Serum/Plasma',NULL),(N'ANTI-HCV-ICT',N'Anti-HCV - ICT',@S,N'Serum/Plasma',NULL),
(N'HIV-ICT',N'HIV 1/2 - ICT',@S,N'Serum/Plasma',NULL),(N'VDRL',N'VDRL',@S,N'Serum',NULL),
(N'RPR',N'RPR',@S,N'Serum',NULL),(N'RF',N'Rheumatoid Factor',@S,N'Serum',N'IU/mL'),
(N'ASO',N'ASO Titer',@S,N'Serum',N'IU/mL'),(N'CRP',N'C-Reactive Protein - Qualitative',@S,N'Serum',NULL),
(N'H-PYLORI-ICT',N'H. pylori Antibody - ICT',@S,N'Serum/Plasma',NULL),(N'DENGUE-NS1-ICT',N'Dengue NS1 - ICT',@S,N'Serum/Plasma',NULL),
(N'DENGUE-IGM-ICT',N'Dengue IgM - ICT',@S,N'Serum/Plasma',NULL),(N'DENGUE-IGG-ICT',N'Dengue IgG - ICT',@S,N'Serum/Plasma',NULL),
(N'MALARIA-ICT',N'Malaria Antigen - ICT',@S,N'Blood',NULL),(N'HBV-HBSAG',N'HBsAg - Non-ICT / ELISA',@S,N'Serum',NULL),
(N'HCV-AB',N'Anti-HCV - Non-ICT / ELISA',@S,N'Serum',NULL),(N'HIV-AB',N'HIV 1/2 - Non-ICT / ELISA',@S,N'Serum',NULL),
-- Immunology
(N'ANA',N'ANA',@I,N'Serum',NULL),(N'ANTI-CCP',N'Anti-CCP',@I,N'Serum',N'U/mL'),
(N'C3',N'Complement C3',@I,N'Serum',N'mg/dL'),(N'C4',N'Complement C4',@I,N'Serum',N'mg/dL'),
(N'IGE',N'Immunoglobulin E',@I,N'Serum',N'kU/L'),(N'IGG',N'Immunoglobulin G',@I,N'Serum',N'mg/dL'),
(N'IGM',N'Immunoglobulin M',@I,N'Serum',N'mg/dL'),
-- Hormones / Special Chemistry
(N'TSH',N'Thyroid Stimulating Hormone',@SC,N'Serum',N'µIU/mL'),(N'FT4',N'Free T4',@SC,N'Serum',N'ng/dL'),
(N'FT3',N'Free T3',@SC,N'Serum',N'pg/mL'),(N'T4',N'Total T4',@SC,N'Serum',N'µg/dL'),
(N'T3',N'Total T3',@SC,N'Serum',N'ng/dL'),(N'LH',N'Luteinizing Hormone',@SC,N'Serum',N'mIU/mL'),
(N'FSH',N'Follicle Stimulating Hormone',@SC,N'Serum',N'mIU/mL'),(N'PROLACTIN',N'Prolactin',@SC,N'Serum',N'ng/mL'),
(N'ESTRADIOL',N'Estradiol / E2',@SC,N'Serum',N'pg/mL'),(N'PROGESTERONE',N'Progesterone',@SC,N'Serum',N'ng/mL'),
(N'TESTOSTERONE',N'Testosterone',@SC,N'Serum',N'ng/dL'),(N'BHCG',N'Beta-hCG',@SC,N'Serum',N'mIU/mL'),
(N'CORTISOL',N'Cortisol',@SC,N'Serum',N'µg/dL'),(N'INSULIN',N'Insulin',@SC,N'Serum',N'µIU/mL'),
(N'PTH',N'Parathyroid Hormone',@SC,N'Serum',N'pg/mL'),(N'VITD',N'Vitamin D - 25 OH',@SC,N'Serum',N'ng/mL'),
(N'B12',N'Vitamin B12',@SC,N'Serum',N'pg/mL'),(N'FERRITIN',N'Ferritin',@SC,N'Serum',N'ng/mL'),
-- Molecular / PCR
(N'HBV-DNA-PCR',N'HBV DNA PCR',@M,N'EDTA Plasma',NULL),(N'HCV-RNA-PCR',N'HCV RNA PCR',@M,N'EDTA Plasma',NULL),
(N'HIV-RNA-PCR',N'HIV RNA PCR',@M,N'EDTA Plasma',NULL),(N'COVID-PCR',N'SARS-CoV-2 RT-PCR',@M,N'Nasopharyngeal Swab',NULL),
(N'HPV-PCR',N'HPV DNA PCR',@M,N'Genital/Cervical Sample',NULL),(N'TB-PCR',N'MTB PCR',@M,N'Sputum',NULL),
-- Clinical pathology
(N'URINE-RE',N'Urine Routine Examination',@CP,N'Urine',NULL),(N'URINE-CULT',N'Urine Culture',@MB,N'Urine',NULL),
(N'URINE-ALB',N'Urine Albumin',@CP,N'Urine',NULL),(N'URINE-SUGAR',N'Urine Sugar',@CP,N'Urine',NULL),
(N'URINE-KETONE',N'Urine Ketone',@CP,N'Urine',NULL),(N'STOOL-RE',N'Stool Routine Examination',@CP,N'Stool',NULL),
(N'OCCULT-BLOOD',N'Occult Blood in Stool',@CP,N'Stool',NULL),(N'PREG-URINE',N'Urine Pregnancy Test',@CP,N'Urine',NULL),
-- Microbiology
(N'SPUTUM-RE',N'Sputum Examination',@MB,N'Sputum',NULL),(N'BLOOD-CULT',N'Blood Culture',@MB,N'Blood',NULL),
(N'WOUND-CULT',N'Wound Culture & Sensitivity',@MB,N'Swab/Pus',NULL),(N'CSF-CULT',N'CSF Culture',@MB,N'CSF',NULL);

INSERT dbo.Tests(TestCode,TestName,DepartmentId,SampleType,Unit,Price)
SELECT t.Code,t.Name,t.Dept,t.Sample,t.Unit,0 FROM @T t
WHERE NOT EXISTS(SELECT 1 FROM dbo.Tests x WHERE x.TestCode=t.Code);
GO