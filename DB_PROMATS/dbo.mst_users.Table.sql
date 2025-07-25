USE [DB_PROMATS]
GO
/****** Object:  Table [dbo].[mst_users]    Script Date: 22/05/2025 20:16:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mst_users](
	[user_id] [int] IDENTITY(1,1) NOT NULL,
	[username] [nvarchar](100) NULL,
	[name] [nvarchar](100) NULL,
	[password] [nvarchar](max) NULL,
	[role] [nvarchar](20) NULL,
	[login_id] [nvarchar](100) NULL,
	[last_update] [datetime2](7) NULL,
 CONSTRAINT [PK_mst_users] PRIMARY KEY CLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[mst_users] ADD  CONSTRAINT [DF_mst_users_last_update]  DEFAULT (getdate()) FOR [last_update]
GO
