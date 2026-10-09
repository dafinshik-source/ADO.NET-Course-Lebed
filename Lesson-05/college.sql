IF DB_ID(N'CollegeDB') IS NULL
    CREATE DATABASE CollegeDB;
GO
USE CollegeDB;
GO

-- 1. Группы
IF OBJECT_ID(N'dbo.Groups', N'U') IS NULL
CREATE TABLE dbo.Groups (
    GroupId   INT IDENTITY(1,1) PRIMARY KEY,
    GroupName NVARCHAR(20) NOT NULL UNIQUE
);
GO

-- 2. Студенты
IF OBJECT_ID(N'dbo.Students', N'U') IS NULL
CREATE TABLE dbo.Students (
    StudentId INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName  NVARCHAR(50) NOT NULL,
    Age       INT NULL,
    GroupId   INT NULL,
    CONSTRAINT FK_Students_Groups
        FOREIGN KEY (GroupId) REFERENCES dbo.Groups(GroupId)
);
GO

-- 3. Преподаватели
IF OBJECT_ID(N'dbo.Teachers', N'U') IS NULL
CREATE TABLE dbo.Teachers (
    TeacherId INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName  NVARCHAR(50) NOT NULL
);
GO

-- 4. Предметы
IF OBJECT_ID(N'dbo.Subjects', N'U') IS NULL
CREATE TABLE dbo.Subjects (
    SubjectId   INT IDENTITY(1,1) PRIMARY KEY,
    SubjectName NVARCHAR(100) NOT NULL UNIQUE,
    TeacherId   INT NULL,
    CONSTRAINT FK_Subjects_Teachers
        FOREIGN KEY (TeacherId) REFERENCES dbo.Teachers(TeacherId)
);
GO

-- 5. Оценки
IF OBJECT_ID(N'dbo.Grades', N'U') IS NULL
CREATE TABLE dbo.Grades (
    GradeId   INT IDENTITY(1,1) PRIMARY KEY,
    StudentId INT NOT NULL,
    SubjectId INT NOT NULL,
    Score     INT NOT NULL CHECK (Score BETWEEN 2 AND 5),
    GradeDate DATE NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Grades_Students
        FOREIGN KEY (StudentId) REFERENCES dbo.Students(StudentId),
    CONSTRAINT FK_Grades_Subjects
        FOREIGN KEY (SubjectId) REFERENCES dbo.Subjects(SubjectId)
);
GO
