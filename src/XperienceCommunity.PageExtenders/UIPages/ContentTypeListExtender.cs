using CMS.DataEngine;
using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.UIPages;
using XperienceCommunity.PageExtenders.UIPages;

[assembly: PageExtender(typeof(ContentTypeListExtender))]

namespace XperienceCommunity.PageExtenders.UIPages;

public class ContentTypeListExtender : PageExtender<ContentTypeList>
{
    public override Task ConfigurePage()
    {
        Page.PageConfiguration.QueryModifiers.AddModifier((query, _) =>
            query.Source(source =>
                source.LeftJoin(sourceExpression: """
                      (SELECT 
                          CI.ContentItemContentTypeID AS Sub_ContentItemContentTypeID,                  
                          Count(*) AS ContentItemCount
                      FROM 
                          CMS_ContentItem CI
                          LEFT JOIN
                          CMS_Class C ON CI.ContentItemContentTypeID = C.ClassID
                      GROUP BY
                        CI.ContentItemContentTypeID) AS ContentItemCounts
                  """,
                condition: "ContentItemCounts.Sub_ContentItemContentTypeID = CMS_Class.ClassID")
            )
            .AddColumn(new QueryColumn("ISNULL(ContentItemCounts.ContentItemCount, '')") { ColumnAlias = "ContentItemCount" }));
        
        Page.PageConfiguration.ColumnConfigurations
            .AddColumn("ContentItemCount", caption: "Number of items");

        return base.ConfigurePage();
    }
}