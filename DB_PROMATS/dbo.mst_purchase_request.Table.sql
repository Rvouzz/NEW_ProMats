USE [DB_PROMATS]
GO
/****** Object:  Table [dbo].[mst_purchase_request]    Script Date: 22/05/2025 20:16:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mst_purchase_request](
	[request_id] [int] IDENTITY(1,1) NOT NULL,
	[material_id] [nvarchar](50) NULL,
	[quantity] [int] NULL,
	[date] [date] NULL,
	[request_by] [nvarchar](50) NULL,
	[status] [nvarchar](50) NULL,
	[part_code] [nvarchar](100) NULL,
	[receipt_proof] [nvarchar](max) NULL,
 CONSTRAINT [PK_mst_purchase_request_1] PRIMARY KEY CLUSTERED 
(
	[request_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[mst_purchase_request] ADD  CONSTRAINT [DF_mst_purchase_request_status]  DEFAULT ('OPEN') FOR [status]
GO
