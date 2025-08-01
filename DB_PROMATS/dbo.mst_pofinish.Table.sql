USE [DB_PROMATS]
GO
/****** Object:  Table [dbo].[mst_pofinish]    Script Date: 22/05/2025 20:16:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mst_pofinish](
	[po_id] [int] IDENTITY(1,1) NOT NULL,
	[part_code] [nvarchar](50) NULL,
	[qty] [int] NULL,
	[date] [date] NULL,
	[status] [nvarchar](50) NULL,
	[request_by] [nvarchar](100) NULL,
	[part_name] [nvarchar](max) NULL,
	[file_order] [nvarchar](max) NULL,
	[receipt_proof] [nvarchar](max) NULL,
 CONSTRAINT [PK_mst_pofinish_1] PRIMARY KEY CLUSTERED 
(
	[po_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[mst_pofinish] ADD  CONSTRAINT [DF_mst_pofinish_date]  DEFAULT (getdate()) FOR [date]
GO
