USE LabMasterDb;
GO
/* Comprehensive profile-to-test assignment. Existing profile/test rows are preserved. */
DECLARE @P TABLE(Code NVARCHAR(30),Name NVARCHAR(150));
INSERT @P VALUES
(N'CBC',N'Complete Blood Count'),(N'LFT',N'Liver Function Test'),(N'RFT',N'Renal Function Test'),
(N'LIPID',N'Lipid Profile'),(N'THYROID',N'Thyroid Profile'),(N'IRON',N'Iron Profile'),
(N'COAG',N'Coagulation Profile'),(N'DENGUE',N'Dengue Profile'),(N'HEPATITIS',N'Hepatitis Screening'),
(N'DIABETIC',N'Diabetes Profile'),(N'FEMALE-HORMONE',N'Female Hormone Profile'),(N'MALE-HORMONE',N'Male Hormone Profile'),
(N'URINE',N'Urine Profile'),(N'STOOL',N'Stool Profile');

INSERT dbo.TestProfiles(ProfileCode,ProfileName)
SELECT p.Code,p.Name FROM @P p WHERE NOT EXISTS(SELECT 1 FROM dbo.TestProfiles x WHERE x.ProfileCode=p.Code);

DECLARE @Map TABLE(ProfileCode NVARCHAR(30),TestCode NVARCHAR(30),DisplayOrder INT);
INSERT @Map VALUES
(N'CBC',N'CBC',1),(N'CBC',N'ESR',2),
(N'LFT',N'ALT',1),(N'LFT',N'AST',2),(N'LFT',N'ALP',3),(N'LFT',N'GGT',4),(N'LFT',N'BILT',5),(N'LFT',N'BILD',6),(N'LFT',N'TP',7),(N'LFT',N'ALB',8),(N'LFT',N'GLOB',9),
(N'RFT',N'UREA',1),(N'RFT',N'CREAT',2),(N'RFT',N'URIC',3),(N'RFT',N'NA',4),(N'RFT',N'K',5),(N'RFT',N'CL',6),(N'RFT',N'CA',7),(N'RFT',N'PHOS',8),
(N'LIPID',N'CHOL',1),(N'LIPID',N'TG',2),(N'LIPID',N'HDL',3),(N'LIPID',N'LDL',4),(N'LIPID',N'VLDL',5),
(N'THYROID',N'TSH',1),(N'THYROID',N'FT4',2),(N'THYROID',N'FT3',3),
(N'IRON',N'FERRITIN',1),(N'IRON',N'B12',2),
(N'COAG',N'PT-INR',1),(N'COAG',N'APTT',2),(N'COAG',N'FIBRINOGEN',3),(N'COAG',N'D-DIMER',4),
(N'DENGUE',N'DENGUE-NS1-ICT',1),(N'DENGUE',N'DENGUE-IGM-ICT',2),(N'DENGUE',N'DENGUE-IGG-ICT',3),
(N'HEPATITIS',N'HBSAG-ICT',1),(N'HEPATITIS',N'ANTI-HCV-ICT',2),
(N'DIABETIC',N'GLU-F',1),(N'DIABETIC',N'GLU-PP',2),(N'DIABETIC',N'HBA1C',3),
(N'FEMALE-HORMONE',N'FSH',1),(N'FEMALE-HORMONE',N'LH',2),(N'FEMALE-HORMONE',N'PROLACTIN',3),(N'FEMALE-HORMONE',N'ESTRADIOL',4),(N'FEMALE-HORMONE',N'PROGESTERONE',5),(N'FEMALE-HORMONE',N'BHCG',6),
(N'MALE-HORMONE',N'TESTOSTERONE',1),(N'MALE-HORMONE',N'FSH',2),(N'MALE-HORMONE',N'LH',3),(N'MALE-HORMONE',N'PROLACTIN',4),
(N'URINE',N'URINE-RE',1),(N'URINE',N'URINE-ALB',2),(N'URINE',N'URINE-SUGAR',3),(N'URINE',N'URINE-KETONE',4),
(N'STOOL',N'STOOL-RE',1),(N'STOOL',N'OCCULT-BLOOD',2);

INSERT dbo.TestProfileItems(ProfileId,TestId,DisplayOrder)
SELECT p.ProfileId,t.TestId,m.DisplayOrder
FROM @Map m JOIN dbo.TestProfiles p ON p.ProfileCode=m.ProfileCode JOIN dbo.Tests t ON t.TestCode=m.TestCode
WHERE NOT EXISTS(SELECT 1 FROM dbo.TestProfileItems x WHERE x.ProfileId=p.ProfileId AND x.TestId=t.TestId);
GO