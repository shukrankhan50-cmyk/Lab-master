USE LabMasterDb;
GO
IF OBJECT_ID('dbo.TestProfileItems','U') IS NOT NULL
BEGIN
 IF NOT EXISTS(SELECT 1 FROM dbo.TestProfileItems pi JOIN dbo.TestProfiles p ON p.ProfileId=pi.ProfileId JOIN dbo.Tests t ON t.TestId=pi.TestId WHERE p.ProfileCode=N'LFT' AND t.TestCode=N'ALT')
  INSERT dbo.TestProfileItems(ProfileId,TestId,DisplayOrder) SELECT p.ProfileId,t.TestId,1 FROM dbo.TestProfiles p CROSS JOIN dbo.Tests t WHERE p.ProfileCode=N'LFT' AND t.TestCode=N'ALT';
END
GO