USE [DB_PROMATS]
GO
/****** Object:  Table [dbo].[mst_accpofinish]    Script Date: 22/05/2025 20:16:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mst_accpofinish](
	[id_acc] [int] IDENTITY(1,1) NOT NULL,
	[po_id] [int] NULL,
	[part_code] [nvarchar](50) NULL,
	[part_name] [nvarchar](max) NULL,
	[qty] [int] NULL,
	[shot] [float] NULL,
	[part] [float] NULL,
	[runner] [float] NULL,
	[date] [date] NULL,
	[request_by] [nvarchar](50) NULL,
	[status] [nvarchar](50) NULL,
	[file_order] [nvarchar](max) NULL,
	[receipt_proof] [nvarchar](max) NULL,
 CONSTRAINT [PK_mst_accpofinish] PRIMARY KEY CLUSTERED 
(
	[id_acc] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[mst_accpofinish] ADD  CONSTRAINT [DF_mst_accpofinish_status]  DEFAULT ('In-progress') FOR [status]
GO
