using DynamicExpressions.Mapping;
using DynamicExpressions.Tests.Mapping.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicExpressions.Tests.Mapping
{
    [TestClass]
    public class MapperTests
    {
        [TestMethod]
        public void ShouldGenerateAndMap()
        {
            var typeDefinition = new DynamicTypeDefinition
            {
                Fields = new List<DynamicFieldDefinition>
                {
                    new DynamicFieldDefinition("Id"),
                    new DynamicFieldDefinition("Name"),
                    new DynamicFieldDefinition("Posts.Count()", "PostCount")
                },
                Lists = new List<DynamicListDefinition>
                {
                    new DynamicListDefinition("Posts")
                    {
                        Fields = new List<DynamicFieldDefinition>
                        {
                            new DynamicFieldDefinition("Title")
                        },
                        Lists = new List<DynamicListDefinition>
                        {
                            new DynamicListDefinition("Tags.OrderBy(Name)",  "Tags")
                            {
                                Fields = new List<DynamicFieldDefinition>
                                {
                                    new DynamicFieldDefinition("Name")
                                }
                            }
                        }
                    },
                    new DynamicListDefinition("Posts.Where(Id > 2).OrderBy(Contents)", "FilteredPosts")
                    {
                        Fields = new List<DynamicFieldDefinition>
                        {
                            new DynamicFieldDefinition("Id"),
                            new DynamicFieldDefinition("Contents")
                        }
                    }
                }
            };

            var generatedType = Mapper.GenerateMappedType<Context, Blog>(typeDefinition);

            var context = new Context
            {
                Blogs = new List<Blog>
                {
                    new Blog { Id = 1, Name = "A", Posts = new List<BlogPost> { new BlogPost { Id = 1, Title = "P1", Contents = "ZContents" } } },
                    new Blog { Id = 2, Name = "B", Posts = new List<BlogPost> {
                        new BlogPost { Id = 2, Title = "P2", Contents = "YContents", Tags = new List<Tag> { new Tag { Name = "T1" } } },
                        new BlogPost { Id = 3, Title = "P3", Contents = "XContents", Tags = new List<Tag> { new Tag { Name = "Z" }, new Tag { Name = "A" } } },
                        new BlogPost { Id = 4, Title = "P4", Contents = "WContents", Tags = new List<Tag> { new Tag { Name = "T4" } } }
                    } }
                }
            };

            var results = context.Blogs.AsQueryable().Select(generatedType.Map(context)).ToList();

            Assert.AreEqual(2, results.Count);

            var r = results[0];
            Assert.AreEqual(1, generatedType.GeneratedType.GetField("Id").GetValue(r));
            Assert.AreEqual("A", generatedType.GeneratedType.GetField("Name").GetValue(r));
            Assert.AreEqual(1, generatedType.GeneratedType.GetField("PostCount").GetValue(r));

            var posts = generatedType.GeneratedType.GetField("Posts").GetValue(r) as List<object>;
            Assert.AreEqual(1, posts.Count);
            var filteredPosts = generatedType.GeneratedType.GetField("FilteredPosts").GetValue(r) as List<object>;
            Assert.AreEqual(0, filteredPosts.Count);

            r = results[1];
            Assert.AreEqual(2, generatedType.GeneratedType.GetField("Id").GetValue(r));
            Assert.AreEqual("B", generatedType.GeneratedType.GetField("Name").GetValue(r));
            Assert.AreEqual(3, generatedType.GeneratedType.GetField("PostCount").GetValue(r));

            posts = generatedType.GeneratedType.GetField("Posts").GetValue(r) as List<object>;
            Assert.AreEqual(3, posts.Count);
            Assert.AreEqual("P2", posts[0].GetType().GetField("Title").GetValue(posts[0]));

            var tags = posts[0].GetType().GetField("Tags").GetValue(posts[1]) as List<object>;
            Assert.AreEqual(2, tags.Count);
            Assert.AreEqual("A", tags[0].GetType().GetField("Name").GetValue(tags[0]));

            filteredPosts = generatedType.GeneratedType.GetField("FilteredPosts").GetValue(r) as List<object>;
            Assert.AreEqual(4, filteredPosts[0].GetType().GetField("Id").GetValue(filteredPosts[0]));
        }
    }
}
