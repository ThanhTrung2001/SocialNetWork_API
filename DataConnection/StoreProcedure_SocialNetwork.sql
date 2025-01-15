CREATE PROCEDURE GetPostById
	(@Id uniqueidentifier)
AS
BEGIN
	SELECT 
		p.Id,
		p.Content,
		p.Post_Type,
		p.Created_At,
		p.In_Group,
		p.Destination_Id,
		p.User_Id,
		p.React_Count,
		u.Email,
		ud.FirstName,
		ud.LastName,
		ud.Avatar,

		a.Id AS Attachment_Id,
		a.Media,
		a.Description,

		s.Id AS Survey_Id,
		s.Expired_At,
		s.Question,
		s.Survey_Type,
		s.Total_Vote,

		si.Id AS SurveyItem_Id,
		si.Option_Name AS SurveyItem_Name,
		si.Total_Vote AS Item_Total,

		udv.User_Id AS Vote_UserId,
		udv.FirstName AS Vote_FirstName,
		udv.LastName AS Vote_LastName,
		udv.Avatar AS Vote_Avatar,
     
		c.Id AS Comment_Id,
		c.Content AS Comment_Content,
		c.Created_At AS Comment_Created_At,
		c.User_Id AS Comment_UserId,
		udc.FirstName AS Comment_FirstName,
		udc.LastName AS Comment_LastName,
		udc.Avatar AS Comment_Avatar,

		urp.React_Type,
		udr.User_Id AS React_UserId,
		udr.FirstName AS React_FirstName,
		udr.LastName AS React_LastName,
		udr.Avatar AS React_Avatar,
		udr.Created_At

	FROM 
		Posts p
	INNER JOIN 
		Users u ON p.User_Id = u.Id
	INNER JOIN 
		User_Details ud ON u.Id = ud.User_Id
	LEFT JOIN
		Post_Attachment pa ON pa.Post_Id = p.Id
	LEFT JOIN
		Attachments a ON pa.Attachment_Id = a.Id
	LEFT JOIN 
		Surveys s ON p.Id = s.Post_Id
	LEFT JOIN 
		Survey_Items si ON s.Id = si.Survey_Id
	LEFT JOIN
		User_SurveyItem_Vote uv ON si.Id = uv.SurveyItem_Id
	LEFT JOIN 
		User_Details udv ON udv.User_Id = uv.User_Id 
	LEFT JOIN
		Comments c ON p.Id = c.Post_Id
	LEFT JOIN
		User_Details udc ON c.User_Id = udc.User_Id
	LEFT JOIN
		User_React_Post urp ON p.Id = urp.Post_Id
	LEFT JOIN
		User_Details udr ON urp.User_Id = udr.User_Id 
	WHERE 
		p.Is_Deleted = 0 AND p.Id = @Id
END
GO


CREATE PROCEDURE GetPosts
AS
BEGIN
	SELECT 
		p.Id,
		p.Content,
		p.Post_Type,
		p.Created_At,
		p.In_Group,
		p.Destination_Id,
		p.User_Id,
		p.React_Count,
		u.Email,
		ud.FirstName,
		ud.LastName,
		ud.Avatar,

		a.Id AS Attachment_Id,
		a.Media,
		a.Description,

		s.Id AS Survey_Id,
		s.Expired_At,
		s.Question,
		s.Survey_Type,
		s.Total_Vote,

		si.Id AS SurveyItem_Id,
		si.Option_Name AS SurveyItem_Name,
		si.Total_Vote AS Item_Total,

		udv.User_Id AS Vote_UserId,
		udv.FirstName AS Vote_FirstName,
		udv.LastName AS Vote_LastName,
		udv.Avatar AS Vote_Avatar,
     
		c.Id AS Comment_Id,
		c.Content AS Comment_Content,
		c.Created_At AS Comment_Created_At,
		c.User_Id AS Comment_UserId,
		udc.FirstName AS Comment_FirstName,
		udc.LastName AS Comment_LastName,
		udc.Avatar AS Comment_Avatar,

		urp.React_Type,
		udr.User_Id AS React_UserId,
		udr.FirstName AS React_FirstName,
		udr.LastName AS React_LastName,
		udr.Avatar AS React_Avatar,
		udr.Created_At

	FROM 
		Posts p
	INNER JOIN 
		Users u ON p.User_Id = u.Id
	INNER JOIN 
		User_Details ud ON u.Id = ud.User_Id
	LEFT JOIN
		Post_Attachment pa ON pa.Post_Id = p.Id
	LEFT JOIN
		Attachments a ON pa.Attachment_Id = a.Id
	LEFT JOIN 
		Surveys s ON p.Id = s.Post_Id
	LEFT JOIN 
		Survey_Items si ON s.Id = si.Survey_Id
	LEFT JOIN
		User_SurveyItem_Vote uv ON si.Id = uv.SurveyItem_Id
	LEFT JOIN 
		User_Details udv ON udv.User_Id = uv.User_Id 
	LEFT JOIN
		Comments c ON p.Id = c.Post_Id
	LEFT JOIN
		User_Details udc ON c.User_Id = udc.User_Id
	LEFT JOIN
		User_React_Post urp ON p.Id = urp.Post_Id
	LEFT JOIN
		User_Details udr ON urp.User_Id = udr.User_Id 
	WHERE 
		p.Is_Deleted = 0
END
GO


CREATE PROCEDURE GetPostsByUserId
 (@Id uniqueidentifier)
AS
BEGIN
	SELECT 
		p.Id,
		p.Content,
		p.Post_Type,
		p.Created_At,
		p.In_Group,
		p.Destination_Id,
		p.User_Id,
		p.React_Count,
		u.Email,
		ud.FirstName,
		ud.LastName,
		ud.Avatar,

		a.Id AS Attachment_Id,
		a.Media,
		a.Description,

		s.Id AS Survey_Id,
		s.Expired_At,
		s.Question,
		s.Survey_Type,
		s.Total_Vote,

		si.Id AS SurveyItem_Id,
		si.Option_Name AS SurveyItem_Name,
		si.Total_Vote AS Item_Total,

		udv.User_Id AS Vote_UserId,
		udv.FirstName AS Vote_FirstName,
		udv.LastName AS Vote_LastName,
		udv.Avatar AS Vote_Avatar,
     
		c.Id AS Comment_Id,
		c.Content AS Comment_Content,
		c.Created_At AS Comment_Created_At,
		c.User_Id AS Comment_UserId,
		udc.FirstName AS Comment_FirstName,
		udc.LastName AS Comment_LastName,
		udc.Avatar AS Comment_Avatar,

		urp.React_Type,
		udr.User_Id AS React_UserId,
		udr.FirstName AS React_FirstName,
		udr.LastName AS React_LastName,
		udr.Avatar AS React_Avatar,
		udr.Created_At

	FROM 
		Posts p
	INNER JOIN 
		Users u ON p.User_Id = u.Id
	INNER JOIN 
		User_Details ud ON u.Id = ud.User_Id
	LEFT JOIN
		Post_Attachment pa ON pa.Post_Id = p.Id
	LEFT JOIN
		Attachments a ON pa.Attachment_Id = a.Id
	LEFT JOIN 
		Surveys s ON p.Id = s.Post_Id
	LEFT JOIN 
		Survey_Items si ON s.Id = si.Survey_Id
	LEFT JOIN
		User_SurveyItem_Vote uv ON si.Id = uv.SurveyItem_Id
	LEFT JOIN 
		User_Details udv ON udv.User_Id = uv.User_Id 
	LEFT JOIN
		Comments c ON p.Id = c.Post_Id
	LEFT JOIN
		User_Details udc ON c.User_Id = udc.User_Id
	LEFT JOIN
		User_React_Post urp ON p.Id = urp.Post_Id
	LEFT JOIN
		User_Details udr ON urp.User_Id = udr.User_Id 
	WHERE 
		p.Is_Deleted = 0 AND p.User_Id = @Id AND p.In_Group = 0
END
GO

CREATE PROCEDURE GetPostsFilter
 (@Id uniqueidentifier, @In_Group bit)
AS
BEGIN
	SELECT 
		p.Id,
		p.Content,
		p.Post_Type,
		p.Created_At,
		p.In_Group,
		p.Destination_Id,
		p.User_Id,
		p.React_Count,
		u.Email,
		ud.FirstName,
		ud.LastName,
		ud.Avatar,

		a.Id AS Attachment_Id,
		a.Media,
		a.Description,

		s.Id AS Survey_Id,
		s.Expired_At,
		s.Question,
		s.Survey_Type,
		s.Total_Vote,

		si.Id AS SurveyItem_Id,
		si.Option_Name AS SurveyItem_Name,
		si.Total_Vote AS Item_Total,

		udv.User_Id AS Vote_UserId,
		udv.FirstName AS Vote_FirstName,
		udv.LastName AS Vote_LastName,
		udv.Avatar AS Vote_Avatar,
     
		c.Id AS Comment_Id,
		c.Content AS Comment_Content,
		c.Created_At AS Comment_Created_At,
		c.User_Id AS Comment_UserId,
		udc.FirstName AS Comment_FirstName,
		udc.LastName AS Comment_LastName,
		udc.Avatar AS Comment_Avatar,

		urp.React_Type,
		udr.User_Id AS React_UserId,
		udr.FirstName AS React_FirstName,
		udr.LastName AS React_LastName,
		udr.Avatar AS React_Avatar,
		udr.Created_At

	FROM 
		Posts p
	INNER JOIN 
		Users u ON p.User_Id = u.Id
	INNER JOIN 
		User_Details ud ON u.Id = ud.User_Id
	LEFT JOIN
		Post_Attachment pa ON pa.Post_Id = p.Id
	LEFT JOIN
		Attachments a ON pa.Attachment_Id = a.Id
	LEFT JOIN 
		Surveys s ON p.Id = s.Post_Id
	LEFT JOIN 
		Survey_Items si ON s.Id = si.Survey_Id
	LEFT JOIN
		User_SurveyItem_Vote uv ON si.Id = uv.SurveyItem_Id
	LEFT JOIN 
		User_Details udv ON udv.User_Id = uv.User_Id 
	LEFT JOIN
		Comments c ON p.Id = c.Post_Id
	LEFT JOIN
		User_Details udc ON c.User_Id = udc.User_Id
	LEFT JOIN
		User_React_Post urp ON p.Id = urp.Post_Id
	LEFT JOIN
		User_Details udr ON urp.User_Id = udr.User_Id 
	WHERE 
		p.Is_Deleted = 0 AND p.In_Group = @In_Group AND p.Destination_Id = @Id
END
GO


CREATE PROCEDURE GetSharePosts
AS
BEGIN
	SELECT 
                            sh.Id,
                            sh.Content AS Share_Content,
                            sh.Created_At AS Share_CreatedAt,
                            sh.Shared_By_User_Id,
                            sh.In_Group AS Share_In_Group,
							sh.React_Count,
                            uds.FirstName AS Share_FirstName,
                            uds.LastName AS Share_LastName,
                            uds.Avatar AS Share_Avatar,
                            
                            p.Id AS Post_Id,
                            p.Content,
                            p.Post_Type,
                            p.Created_At,
                            p.In_Group,
                            p.Destination_Id,
                            p.User_Id,
                            u.Email,
                            ud.FirstName,
                            ud.LastName,
                            ud.Avatar,

                            a.Id AS Attachment_Id,
                            a.Media,
                            a.Description,

                            s.Id AS Survey_Id,
                            s.Expired_At,
                            s.Question,
                            s.Survey_Type,
                            s.Total_Vote,

                            si.Id AS SurveyItem_Id,
                            si.Option_Name AS SurveyItem_Name,
                            si.Total_Vote AS Item_Total,

                            udv.User_Id AS Vote_UserId,
                            udv.FirstName AS Vote_FirstName,
                            udv.LastName AS Vote_LastName,
                            udv.Avatar AS Vote_Avatar,

                            c.Id AS Comment_Id,
                            c.Content AS Comment_Content,
                            c.Created_At AS Comment_Created_At,
                            c.User_Id AS Comment_UserId,
                            udc.FirstName AS Comment_FirstName,
                            udc.LastName AS Comment_LastName,
                            udc.Avatar AS Comment_Avatar,

                            urp.React_Type,
                            udr.User_Id AS React_UserId,
                            udr.FirstName AS React_FirstName,
                            udr.LastName AS React_LastName,
                            udr.Avatar AS React_Avatar,
                            udr.Created_At

                         FROM Share_Posts sh
                         INNER JOIN 
                            User_Details uds ON sh.Shared_By_User_Id = uds.User_Id
                         LEFT JOIN 
                            Posts p ON p.Id = sh.Shared_Post_Id
                         LEFT JOIN 
                            Users u ON p.User_Id = u.Id
                        LEFT JOIN 
                            User_Details ud ON u.Id = ud.User_Id
                        LEFT JOIN
                            Post_Attachment pa ON pa.Post_Id = p.Id
                        LEFT JOIN
                            Attachments a ON pa.Attachment_Id = a.Id
                        LEFT JOIN 
                            Surveys s ON p.Id = s.Post_Id
                        LEFT JOIN 
                            Survey_Items si ON s.Id = si.Survey_Id
                        LEFT JOIN
                            User_SurveyItem_Vote uv ON si.Id = uv.SurveyItem_Id
                        LEFT JOIN 
                            User_Details udv ON udv.User_Id = uv.User_Id
                        LEFT JOIN
                            Comments c ON sh.Id = c.Post_Id
                        LEFT JOIN
                            User_Details udc ON c.User_Id = udc.User_Id
                        LEFT JOIN
                            User_React_Post urp ON sh.Id = urp.Post_Id
                        LEFT JOIN
                            User_Details udr ON urp.User_Id = udr.User_Id
                        WHERE 
                            sh.Is_Deleted = 0
END
GO


CREATE PROCEDURE GetSharePostsByUserId
 (@Id uniqueidentifier)
AS
BEGIN
	SELECT 
                            sh.Id,
                            sh.Content AS Share_Content,
                            sh.Created_At AS Share_CreatedAt,
                            sh.Shared_By_User_Id,
                            sh.In_Group AS Share_In_Group,
							sh.React_Count,
                            uds.FirstName AS Share_FirstName,
                            uds.LastName AS Share_LastName,
                            uds.Avatar AS Share_Avatar,
                            
                            p.Id AS Post_Id,
                            p.Content,
                            p.Post_Type,
                            p.Created_At,
                            p.In_Group,
                            p.Destination_Id,
                            p.User_Id,
                            u.Email,
                            ud.FirstName,
                            ud.LastName,
                            ud.Avatar,

                            a.Id AS Attachment_Id,
                            a.Media,
                            a.Description,

                            s.Id AS Survey_Id,
                            s.Expired_At,
                            s.Question,
                            s.Survey_Type,
                            s.Total_Vote,

                            si.Id AS SurveyItem_Id,
                            si.Option_Name AS SurveyItem_Name,
                            si.Total_Vote AS Item_Total,

                            udv.User_Id AS Vote_UserId,
                            udv.FirstName AS Vote_FirstName,
                            udv.LastName AS Vote_LastName,
                            udv.Avatar AS Vote_Avatar,

							c.Id AS Comment_Id,
                            c.Content AS Comment_Content,
                            c.Created_At AS Comment_Created_At,
                            c.User_Id AS Comment_UserId,
                            udc.FirstName AS Comment_FirstName,
                            udc.LastName AS Comment_LastName,
                            udc.Avatar AS Comment_Avatar,

                            urp.React_Type,
                            udr.User_Id AS React_UserId,
                            udr.FirstName AS React_FirstName,
                            udr.LastName AS React_LastName,
                            udr.Avatar AS React_Avatar,
                            udr.Created_At

                         FROM Share_Posts sh
                         INNER JOIN 
                            User_Details uds ON sh.Shared_By_User_Id = uds.User_Id
                         LEFT JOIN 
                            Posts p ON p.Id = sh.Shared_Post_Id
                         LEFT JOIN 
                            Users u ON p.User_Id = u.Id
                         INNER JOIN 
                            User_Details ud ON u.Id = ud.User_Id
                         LEFT JOIN
                            Post_Attachment pa ON pa.Post_Id = p.Id
                         LEFT JOIN
                            Attachments a ON pa.Attachment_Id = a.Id
                         LEFT JOIN 
                            Surveys s ON p.Id = s.Post_Id
                         LEFT JOIN 
                            Survey_Items si ON s.Id = si.Survey_Id
                         LEFT JOIN
                            User_SurveyItem_Vote uv ON si.Id = uv.SurveyItem_Id
                         LEFT JOIN 
                            User_Details udv ON udv.User_Id = uv.User_Id 
                         LEFT JOIN
                            Comments c ON sh.Id = c.Post_Id
                         LEFT JOIN
                            User_Details udc ON c.User_Id = udc.User_Id
                         LEFT JOIN
                            User_React_Post urp ON sh.Id = urp.Post_Id
                         LEFT JOIN
                            User_Details udr ON urp.User_Id = udr.User_Id 
                         WHERE sh.Is_Deleted = 0 AND sh.Shared_By_User_Id = @Id
END
GO

