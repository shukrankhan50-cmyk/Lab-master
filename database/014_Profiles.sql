USE LabMasterDb;
GO
IF NOT EXISTS(SELECT 1 FROM dbo.TestProfiles WHERE ProfileCode=N'LFT') INSERT dbo.TestProfiles(ProfileCode,ProfileName) VALUES(N'LFT',N'Liver Function Test');
IF NOT EXISTS(SELECT 1 FROM dbo.TestProfiles WHERE ProfileCode=N'RFT') INSERT dbo.TestProfiles(ProfileCode,ProfileName) VALUES(N'RFT',N'Renal Function Test');
IF NOT EXISTS(SELECT 1 FROM dbo.TestProfiles WHERE ProfileCode=N'THYROID') INSERT dbo.TestProfiles(ProfileCode,ProfileName) VALUES(N'THYROID',N'Thyroid Profile');
IF NOT EXISTS(SELECT 1 FROM dbo.TestProfiles WHERE ProfileCode=N'LIPID') INSERT dbo.TestProfiles(ProfileCode,ProfileName) VALUES(N'LIPID',N'Lipid Profile');
GO