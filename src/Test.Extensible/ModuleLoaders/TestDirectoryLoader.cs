using Extensible.Loaders;
using System;
using System.IO;
using System.Linq;
using Test.Extensible.Shared;
using Xunit;

namespace Test.Extensible.ModuleLoaders
{
    public class TestDirectoryModuleLoader
    {
        [Fact]
        public void DirectoryLoader_ShouldThrowException_IfDirectoryIsInvalid()
        {
            //Given 
            var loader = new DirectoryLoader<TestAppEvents>(string.Empty);
            var events = new TestAppEvents();

            // When & Then
            Assert.Throws<ArgumentException>(() => loader.Load(events));
        }

        [Fact]
        public void DirectoryLoader_ShouldLoadModules_IfDirectoryIsValid()
        {
            //Given
            var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "Test.Modules", "Path1");

            var loader = new DirectoryLoader<TestAppEvents>(path);
            var events = new TestAppEvents();

            //When
            var modules = loader.Load(events);

            //Then
            Assert.True(modules.Any());
        }

        [Fact]
        public void DirectoryLoader_ShouldLoadSubDirectories_IfRecurseiveIsSet()
        {
            //Given
            var path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;

            var loader = new DirectoryLoader<TestAppEvents>(path, true);
            var events = new TestAppEvents();

            //When
            var modules = loader.Load(events);

            //Then
            Assert.True(modules.Any());
        }

        [Fact]
        public void DirectoryLoader_ShouldLoadModules_IfModuleFilterIsValid()
        {
            //Given
            var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "Test.Modules", "Path3");

            var loader = new DirectoryLoader<TestAppEvents>(path, moduleFilter: "*.mod");
            var events = new TestAppEvents();

            //When
            var modules = loader.Load(events);

            //Then
            Assert.True(modules.Any());
        }

        [Fact]
        public void DirectoryLoader_ShouldLoadAllModules_IfMultipleDirectoriesAreProvided()
        {
            //Given
            var path1 = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "Test.Modules", "Path1");
            var path2 = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "Test.Modules", "Path2");

            var loader = new DirectoryLoader<TestAppEvents>(new string[] { path1, path2 });
            var events = new TestAppEvents();

            //When
            var modules = loader.Load(events);

            //Then
            Assert.Equal(2, modules.Count());
        }

        [Fact]
        public void DirectoryLoader_ShouldBindEvents_IfDirectoryIsValid()
        {
            //Given
            var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "Test.Modules", "Path1");

            var loader = new DirectoryLoader<TestAppEvents>(path);
            var events = new TestAppEvents();

            //When
            var modules = loader.Load(events);

            //Then
            Assert.NotNull(events.MyEvent);
        }

        [Fact]
        public void DirectoryLoader_ShouldNotBindEvents_WhenNoValidTypeNamesAreProvided()
        {
            // Given
            var path = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "Test.Modules", "Path4");

            var loader = new DirectoryLoader<TestAppEvents>(path);
            var events = new TestAppEvents();

            // When
            var modules = loader.Load(events);

            //Then
            Assert.Null(events.MyEvent);
        }
    }
}