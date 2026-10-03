USE LabMasterDb;
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Tests WHERE TestCode=N'CBC')
INSERT dbo.Tests(TestCode,TestName,DepartmentId,SampleType,Unit,ReferenceRange,Price)
SELECT N'CBC',N'Complete Blood Count',DepartmentId,N'EDTA Blood',NULL,NULL,500 FROM dbo.Departments WHERE DepartmentName=N'Hematology';
IF NOT EXISTS (SELECT 1 FROM dbo.Tests WHERE TestCode=N'URINE-RE')
INSERT dbo.Tests(TestCode,TestName,DepartmentId,SampleType,Unit,ReferenceRange,Price)
SELECT N'URINE-RE',N'Urine Routine Examination',DepartmentId,N'Urine',NULL,NULL,300 FROM dbo.Departments WHERE DepartmentName=N'Clinical Pathology';
IF NOT EXISTS (SELECT 1 FROM dbo.Tests WHERE TestCode=N'TSH')
INSERT dbo.Tests(TestCode,TestName,DepartmentId,SampleType,Unit,ReferenceRange,Price)
SELECT N'TSH',N'Thyroid Stimulating Hormone',DepartmentId,N'Serum',N'µIU/mL',N'Lab-specific',700 FROM dbo.Departments WHERE DepartmentName=N'Serology / Immunology';
IF NOT EXISTS (SELECT 1 FROM dbo.Tests WHERE TestCode=N'ALT')
INSERT dbo.Tests(TestCode,TestName,DepartmentId,SampleType,Unit,ReferenceRange,Price)
SELECT N'ALT',N'ALT / SGPT',DepartmentId,N'Serum',N'U/L',N'Lab-specific',400 FROM dbo.Departments WHERE DepartmentName=N'Biochemistry';
GO