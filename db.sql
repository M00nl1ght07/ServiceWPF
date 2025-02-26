-- Создание базы данных
CREATE DATABASE ServiceDesk;
GO

USE ServiceDesk;
GO

-- Создание таблиц
CREATE TABLE Roles (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(255)
);

CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Login NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    MiddleName NVARCHAR(50),
    RoleID INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);

CREATE TABLE RequestStatuses (
    StatusID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(255)
);

CREATE TABLE RequestPriorities (
    PriorityID INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(255)
);

CREATE TABLE Requests (
    RequestID INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    CreatedByUserID INT NOT NULL,
    ExecutorID INT,
    StatusID INT NOT NULL,
    PriorityID INT NOT NULL,
    CompletionDate DATETIME,
    LastModifiedDate DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (CreatedByUserID) REFERENCES Users(UserID),
    FOREIGN KEY (ExecutorID) REFERENCES Users(UserID),
    FOREIGN KEY (StatusID) REFERENCES RequestStatuses(StatusID),
    FOREIGN KEY (PriorityID) REFERENCES RequestPriorities(PriorityID)
);

CREATE TABLE RequestHistory (
    HistoryID INT PRIMARY KEY IDENTITY(1,1),
    RequestID INT NOT NULL,
    StatusID INT NOT NULL,
    ChangedByUserID INT NOT NULL,
    ChangeDate DATETIME NOT NULL DEFAULT GETDATE(),
    Comment NVARCHAR(MAX),
    FOREIGN KEY (RequestID) REFERENCES Requests(RequestID),
    FOREIGN KEY (StatusID) REFERENCES RequestStatuses(StatusID),
    FOREIGN KEY (ChangedByUserID) REFERENCES Users(UserID)
);

CREATE TABLE RequestComments (
    CommentID INT PRIMARY KEY IDENTITY(1,1),
    RequestID INT NOT NULL,
    UserID INT NOT NULL,
    Text NVARCHAR(MAX) NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (RequestID) REFERENCES Requests(RequestID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Заполнение справочных таблиц
INSERT INTO Roles (Name, Description) VALUES 
('Admin', N'Администратор системы'),
('Executor', N'Исполнитель заявок'),
('User', N'Пользователь системы');

INSERT INTO RequestStatuses (Name, Description) VALUES
(N'Новая', N'Заявка создана'),
(N'В работе', N'Заявка принята в работу'),
(N'На проверке', N'Заявка ожидает проверки'),
(N'Завершена', N'Заявка выполнена'),
(N'Отменена', N'Заявка отменена');

INSERT INTO RequestPriorities (Name, Description) VALUES
(N'Низкий', N'Низкий приоритет'),
(N'Средний', N'Средний приоритет'),
(N'Высокий', N'Высокий приоритет');

-- Создание индексов для оптимизации запросов
CREATE INDEX IX_Requests_CreatedByUserID ON Requests(CreatedByUserID);
CREATE INDEX IX_Requests_ExecutorID ON Requests(ExecutorID);
CREATE INDEX IX_Requests_StatusID ON Requests(StatusID);
CREATE INDEX IX_RequestHistory_RequestID ON RequestHistory(RequestID);
CREATE INDEX IX_RequestComments_RequestID ON RequestComments(RequestID);