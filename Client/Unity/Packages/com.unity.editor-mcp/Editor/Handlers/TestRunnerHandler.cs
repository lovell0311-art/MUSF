using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditorMCP.Helpers;
using Newtonsoft.Json.Linq;

namespace UnityEditorMCP.Handlers
{
    /// <summary>
    /// Handles Unity Test Runner operations for executing and managing tests
    /// </summary>
    public static class TestRunnerHandler
    {
        private static TestRunnerApi testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
        private static TestRunCallback currentCallback;
        private static Dictionary<string, TestResult> lastTestResults = new Dictionary<string, TestResult>();
        private static bool isRunningTests = false;

        private enum RequestedTestMode
        {
            All,
            EditMode,
            PlayMode
        }
        
        /// <summary>
        /// Lists all available tests in the project
        /// </summary>
        public static object ListTests(JObject parameters)
        {
            try
            {
                var requestedMode = ParseRequestedTestMode(parameters["testMode"]?.ToString());
                var filterPattern = parameters["filter"]?.ToString();
                var includeCategories = parameters["includeCategories"]?.ToObject<string[]>();
                var excludeCategories = parameters["excludeCategories"]?.ToObject<string[]>();
                
                var tests = new List<object>();
                
                // Retrieve test data through reflection (Unity's API doesn't expose test listing directly)
                var editorAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => a.GetReferencedAssemblies().Any(r => r.Name == "nunit.framework"));
                
                foreach (var assembly in editorAssemblies)
                {
                    try
                    {
                        var types = assembly.GetTypes()
                            .Where(t => HasAttribute(t, "TestFixtureAttribute") ||
                                       t.GetMethods().Any(IsTestMethod));
                        
                        foreach (var type in types)
                        {
                            var testMethods = type.GetMethods().Where(IsTestMethod).ToArray();
                            if (testMethods.Length == 0)
                                continue;

                            // Check if this is an Edit Mode or Play Mode test
                            bool isPlayMode = testMethods.Any(m => HasAttribute(m, nameof(UnityTestAttribute)));
                            TestMode typeTestMode = isPlayMode ? TestMode.PlayMode : TestMode.EditMode;
                            
                            if (!MatchesRequestedMode(requestedMode, typeTestMode))
                                continue;
                            
                            foreach (var method in testMethods)
                            {
                                var testName = $"{type.FullName}.{method.Name}";
                                
                                // Apply filter if specified
                                if (!string.IsNullOrEmpty(filterPattern) &&
                                    testName.IndexOf(filterPattern, StringComparison.OrdinalIgnoreCase) < 0)
                                    continue;
                                
                                // Get categories
                                var categories = GetCategoryNames(method).ToArray();
                                
                                // Apply category filters
                                if (includeCategories != null && includeCategories.Length > 0)
                                {
                                    if (!categories.Any(c => includeCategories.Contains(c)))
                                        continue;
                                }
                                
                                if (excludeCategories != null && excludeCategories.Length > 0)
                                {
                                    if (categories.Any(c => excludeCategories.Contains(c)))
                                        continue;
                                }
                                
                                tests.Add(new
                                {
                                    name = testName,
                                    methodName = method.Name,
                                    className = type.FullName,
                                    assemblyName = assembly.GetName().Name,
                                    testMode = typeTestMode.ToString(),
                                    categories = categories,
                                    isAsync = HasAttribute(method, nameof(UnityTestAttribute))
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Failed to process assembly {assembly.GetName().Name}: {ex.Message}");
                    }
                }
                
                return new
                {
                    tests = tests.ToArray(),
                    totalCount = tests.Count,
                    testMode = RequestedModeToString(requestedMode),
                    message = $"Found {tests.Count} tests"
                };
            }
            catch (Exception ex)
            {
                return Response.Error($"Failed to list tests: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Runs specified tests or all tests
        /// </summary>
        public static object RunTests(JObject parameters)
        {
            try
            {
                if (isRunningTests)
                {
                    return Response.Error("Tests are already running. Please wait for them to complete or cancel.");
                }
                
                var requestedMode = ParseRequestedTestMode(parameters["testMode"]?.ToString());
                var testNames = parameters["testNames"]?.ToObject<string[]>();
                var runAll = parameters["runAll"]?.ToObject<bool>() ?? false;
                var includeCategories = parameters["includeCategories"]?.ToObject<string[]>();
                var excludeCategories = parameters["excludeCategories"]?.ToObject<string[]>();
                
                // Create filter for test execution
                var filter = new Filter()
                {
                    testMode = ToUnityTestMode(requestedMode),
                    testNames = testNames,
                    categoryNames = includeCategories
                };
                
                // Clear previous results
                lastTestResults.Clear();
                
                // Create callback handler
                if (currentCallback == null)
                {
                    currentCallback = new TestRunCallback();
                    testRunnerApi.RegisterCallbacks(currentCallback);
                }
                
                isRunningTests = true;
                
                // Execute tests
                var executionSettings = new ExecutionSettings(filter);
                testRunnerApi.Execute(executionSettings);
                
                return new
                {
                    message = "Test execution started",
                    testMode = RequestedModeToString(requestedMode),
                    resolvedTestMode = filter.testMode.ToString(),
                    testCount = testNames?.Length ?? 0,
                    runAll = runAll,
                    excludedCategoriesIgnored = excludeCategories != null && excludeCategories.Length > 0,
                    timestamp = DateTime.UtcNow.ToString("o")
                };
            }
            catch (Exception ex)
            {
                isRunningTests = false;
                return Response.Error($"Failed to run tests: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Gets the results of the last test run
        /// </summary>
        public static object GetTestResults(JObject parameters)
        {
            try
            {
                var includeDetails = parameters["includeDetails"]?.ToObject<bool>() ?? true;
                var filterStatus = parameters["filterStatus"]?.ToString();
                
                if (lastTestResults.Count == 0)
                {
                    return new
                    {
                        message = "No test results available. Run tests first.",
                        hasResults = false
                    };
                }
                
                var results = new List<object>();
                
                foreach (var kvp in lastTestResults)
                {
                    var result = kvp.Value;
                    
                    // Apply status filter if specified
                    if (!string.IsNullOrEmpty(filterStatus))
                    {
                        if (!result.Status.ToString().Equals(filterStatus, StringComparison.OrdinalIgnoreCase))
                            continue;
                    }
                    
                    var testResult = new
                    {
                        name = kvp.Key,
                        status = result.Status.ToString(),
                        duration = result.Duration,
                        startTime = result.StartTime.ToString("o"),
                        endTime = result.EndTime.ToString("o")
                    };
                    
                    if (includeDetails)
                    {
                        var detailedResult = new
                        {
                            name = kvp.Key,
                            status = result.Status.ToString(),
                            duration = result.Duration,
                            startTime = result.StartTime.ToString("o"),
                            endTime = result.EndTime.ToString("o"),
                            message = result.Message,
                            stackTrace = result.StackTrace,
                            output = result.Output
                        };
                        results.Add(detailedResult);
                    }
                    else
                    {
                        results.Add(testResult);
                    }
                }
                
                var summary = CalculateTestSummary();
                
                return new
                {
                    results = results.ToArray(),
                    summary = summary,
                    isRunning = isRunningTests,
                    totalTests = lastTestResults.Count,
                    message = "Test results retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return Response.Error($"Failed to get test results: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Cancels currently running tests
        /// </summary>
        public static object CancelTests(JObject parameters)
        {
            try
            {
                if (!isRunningTests)
                {
                    return new
                    {
                        message = "No tests are currently running",
                        wasCancelled = false
                    };
                }
                
                // Unity doesn't provide a direct way to cancel tests, but we can try to stop the test runner
                EditorApplication.isPlaying = false;
                isRunningTests = false;
                
                return new
                {
                    message = "Test cancellation requested",
                    wasCancelled = true,
                    timestamp = DateTime.UtcNow.ToString("o")
                };
            }
            catch (Exception ex)
            {
                return Response.Error($"Failed to cancel tests: {ex.Message}");
            }
        }
        
        #region Helper Methods
        
        private static RequestedTestMode ParseRequestedTestMode(string mode)
        {
            if (string.IsNullOrEmpty(mode))
                return RequestedTestMode.All;
            
            if (string.Equals(mode, "all", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(mode, "EditAndPlayMode", StringComparison.OrdinalIgnoreCase))
            {
                return RequestedTestMode.All;
            }
            
            if (Enum.TryParse<TestMode>(mode, true, out TestMode result) && result == TestMode.PlayMode)
                return RequestedTestMode.PlayMode;
            
            return RequestedTestMode.EditMode;
        }

        private static TestMode ToUnityTestMode(RequestedTestMode requestedMode)
        {
            return requestedMode == RequestedTestMode.PlayMode ? TestMode.PlayMode : TestMode.EditMode;
        }

        private static bool MatchesRequestedMode(RequestedTestMode requestedMode, TestMode actualMode)
        {
            return requestedMode == RequestedTestMode.All || ToUnityTestMode(requestedMode) == actualMode;
        }

        private static string RequestedModeToString(RequestedTestMode requestedMode)
        {
            switch (requestedMode)
            {
                case RequestedTestMode.EditMode:
                    return TestMode.EditMode.ToString();
                case RequestedTestMode.PlayMode:
                    return TestMode.PlayMode.ToString();
                default:
                    return "All";
            }
        }

        private static bool IsTestMethod(MethodInfo method)
        {
            return HasAttribute(method, "TestAttribute") || HasAttribute(method, nameof(UnityTestAttribute));
        }

        private static bool HasAttribute(MemberInfo member, string attributeName)
        {
            return member.GetCustomAttributes(true)
                .Any(attribute => string.Equals(attribute.GetType().Name, attributeName, StringComparison.Ordinal));
        }

        private static IEnumerable<string> GetCategoryNames(MemberInfo member)
        {
            return member.GetCustomAttributes(true)
                .Where(attribute => string.Equals(attribute.GetType().Name, "CategoryAttribute", StringComparison.Ordinal))
                .Select(attribute =>
                {
                    var property = attribute.GetType().GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
                    return property != null ? property.GetValue(attribute, null) as string : null;
                })
                .Where(name => !string.IsNullOrEmpty(name));
        }
        
        private static object CalculateTestSummary()
        {
            int passed = 0;
            int failed = 0;
            int skipped = 0;
            int inconclusive = 0;
            double totalDuration = 0;
            
            foreach (var result in lastTestResults.Values)
            {
                totalDuration += result.Duration;
                
                switch (result.Status)
                {
                    case TestStatus.Passed:
                        passed++;
                        break;
                    case TestStatus.Failed:
                        failed++;
                        break;
                    case TestStatus.Skipped:
                        skipped++;
                        break;
                    case TestStatus.Inconclusive:
                        inconclusive++;
                        break;
                }
            }
            
            return new
            {
                total = lastTestResults.Count,
                passed = passed,
                failed = failed,
                skipped = skipped,
                inconclusive = inconclusive,
                duration = totalDuration,
                successRate = lastTestResults.Count > 0 ? (passed / (double)lastTestResults.Count) * 100 : 0
            };
        }
        
        #endregion
        
        /// <summary>
        /// Internal class to handle test execution callbacks
        /// </summary>
        private class TestRunCallback : ICallbacks
        {
            public void RunStarted(ITestAdaptor testsToRun)
            {
                Debug.Log($"[TestRunner] Starting test run");
                lastTestResults.Clear();
            }
            
            public void RunFinished(ITestResultAdaptor result)
            {
                Debug.Log($"[TestRunner] Test run completed");
                ProcessTestResults(result);
                isRunningTests = false;
            }
            
            public void TestStarted(ITestAdaptor test)
            {
                Debug.Log($"[TestRunner] Test started: {test.FullName}");
            }
            
            public void TestFinished(ITestResultAdaptor result)
            {
                Debug.Log($"[TestRunner] Test finished: {result.Test.FullName} - {result.TestStatus}");
                
                // Store individual test result
                var testResult = new TestResult
                {
                    Name = result.Test.FullName,
                    Status = ConvertTestStatus(result.TestStatus),
                    Duration = result.Duration,
                    StartTime = result.StartTime,
                    EndTime = result.EndTime,
                    Message = result.Message,
                    StackTrace = result.StackTrace,
                    Output = result.Output
                };
                
                lastTestResults[result.Test.FullName] = testResult;
            }
            
            private void ProcessTestResults(ITestResultAdaptor result)
            {
                // Process all results recursively
                if (result.HasChildren)
                {
                    foreach (var child in result.Children)
                    {
                        ProcessTestResults(child);
                    }
                }
                else if (result.Test != null)
                {
                    // Leaf node - actual test
                    var testResult = new TestResult
                    {
                        Name = result.Test.FullName,
                        Status = ConvertTestStatus(result.TestStatus),
                        Duration = result.Duration,
                        StartTime = result.StartTime,
                        EndTime = result.EndTime,
                        Message = result.Message,
                        StackTrace = result.StackTrace,
                        Output = result.Output
                    };
                    
                    lastTestResults[result.Test.FullName] = testResult;
                }
            }
            
            private TestStatus ConvertTestStatus(UnityEditor.TestTools.TestRunner.Api.TestStatus status)
            {
                switch (status)
                {
                    case UnityEditor.TestTools.TestRunner.Api.TestStatus.Passed:
                        return TestStatus.Passed;
                    case UnityEditor.TestTools.TestRunner.Api.TestStatus.Failed:
                        return TestStatus.Failed;
                    case UnityEditor.TestTools.TestRunner.Api.TestStatus.Skipped:
                        return TestStatus.Skipped;
                    case UnityEditor.TestTools.TestRunner.Api.TestStatus.Inconclusive:
                        return TestStatus.Inconclusive;
                    default:
                        return TestStatus.Inconclusive;
                }
            }
        }
        
        /// <summary>
        /// Internal class to store test results
        /// </summary>
        private class TestResult
        {
            public string Name { get; set; }
            public TestStatus Status { get; set; }
            public double Duration { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public string Message { get; set; }
            public string StackTrace { get; set; }
            public string Output { get; set; }
        }
        
        private enum TestStatus
        {
            Passed,
            Failed,
            Skipped,
            Inconclusive
        }
    }
}
