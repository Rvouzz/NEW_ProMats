USE [DB_PROMATS]
GO
/****** Object:  Table [dbo].[mst_part]    Script Date: 22/05/2025 20:16:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mst_part](
	[id_part] [int] IDENTITY(1,1) NOT NULL,
	[part_code] [nvarchar](100) NULL,
	[part_name] [nvarchar](max) NULL,
	[part] [float] NULL,
	[runner] [float] NULL,
	[record_date] [datetime2](7) NULL,
 CONSTRAINT [PK_Table_1] PRIMARY KEY CLUSTERED 
(
	[id_part] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[mst_part] ADD  CONSTRAINT [DF_Table_1_record_date]  DEFAULT (getdate()) FOR [record_date]
GO
