USE [DB_PROMATS]
GO
/****** Object:  Table [dbo].[tbl_detail_pr]    Script Date: 22/05/2025 20:16:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_detail_pr](
	[request_id] [int] NULL,
	[supplier_name] [nvarchar](255) NULL,
	[lead_time] [nvarchar](100) NULL,
	[shipment] [nvarchar](100) NULL,
	[total_time] [nvarchar](100) NULL,
	[acc_by] [nvarchar](100) NULL,
	[date] [datetime2](7) NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tbl_detail_pr] ADD  CONSTRAINT [DF_tbl_detail_pr_date]  DEFAULT (getdate()) FOR [date]
GO
