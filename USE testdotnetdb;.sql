USE testdotnetdb;
GO
SELECT * FROM DeliveryChallanHeaderCreation; 
GO

USE testdotnetdb;
GO
SELECT * FROM DeliveryChallanDetailCreation; 
GO


USE testdotnetdb;
GO
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE';
GO



CREATE TABLE Students(
    ID INT PRIMARY KEY,
    Name VARCHAR(50),
    Age INT
);
GO

INSERT INTO Students VALUES (1, 'Alice', 20);
INSERT INTO Students VALUES (2, 'Bob', 22);
GO
USE testdb2;
GO

SELECT * FROM Students;
GO




USE testdb;
GO
CREATE PROCEDURE InsertStudent
    @ID INT,
    @Name NVARCHAR(50),
    @Age INT
AS
BEGIN
    INSERT INTO Students (ID, Name, Age)
    VALUES (@ID, @Name, @Age);
END;
GO
