USE [DB_PROMATS]
GO
/****** Object:  Table [dbo].[tbl_detail_finish]    Script Date: 22/05/2025 20:16:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_detail_finish](
	[id_detail] [int] IDENTITY(1,1) NOT NULL,
	[po_id] [int] NULL,
	[deliver_by] [nvarchar](100) NULL,
	[finish_date] [datetime2](7) NULL,
	[remark] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_detail_finish] PRIMARY KEY CLUSTERED 
(
	[id_detail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[tbl_detail_finish] ADD  CONSTRAINT [DF_tbl_detail_finish_finish_date]  DEFAULT (getdate()) FOR [finish_date]
GO
