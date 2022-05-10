namespace ADOCore.Models
{
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010", IsNullable = false)]
    public partial class TestRun
    {

        private TestRunTimes timesField;

        private TestRunTestSettings testSettingsField;

        private TestRunUnitTestResult[] resultsField;

        private TestRunUnitTest[] testDefinitionsField;

        private TestRunTestEntry[] testEntriesField;

        private TestRunTestList[] testListsField;

        private TestRunResultSummary resultSummaryField;

        private string idField;

        private string nameField;

        private string runUserField;

        /// <remarks/>
        public TestRunTimes Times
        {
            get
            {
                return this.timesField;
            }
            set
            {
                this.timesField = value;
            }
        }

        /// <remarks/>
        public TestRunTestSettings TestSettings
        {
            get
            {
                return this.testSettingsField;
            }
            set
            {
                this.testSettingsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("UnitTestResult", IsNullable = false)]
        public TestRunUnitTestResult[] Results
        {
            get
            {
                return this.resultsField;
            }
            set
            {
                this.resultsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("UnitTest", IsNullable = false)]
        public TestRunUnitTest[] TestDefinitions
        {
            get
            {
                return this.testDefinitionsField;
            }
            set
            {
                this.testDefinitionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("TestEntry", IsNullable = false)]
        public TestRunTestEntry[] TestEntries
        {
            get
            {
                return this.testEntriesField;
            }
            set
            {
                this.testEntriesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("TestList", IsNullable = false)]
        public TestRunTestList[] TestLists
        {
            get
            {
                return this.testListsField;
            }
            set
            {
                this.testListsField = value;
            }
        }

        /// <remarks/>
        public TestRunResultSummary ResultSummary
        {
            get
            {
                return this.resultSummaryField;
            }
            set
            {
                this.resultSummaryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string runUser
        {
            get
            {
                return this.runUserField;
            }
            set
            {
                this.runUserField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunTimes
    {

        private System.DateTime creationField;

        private System.DateTime queuingField;

        private System.DateTime startField;

        private System.DateTime finishField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime creation
        {
            get
            {
                return this.creationField;
            }
            set
            {
                this.creationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime queuing
        {
            get
            {
                return this.queuingField;
            }
            set
            {
                this.queuingField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime start
        {
            get
            {
                return this.startField;
            }
            set
            {
                this.startField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime finish
        {
            get
            {
                return this.finishField;
            }
            set
            {
                this.finishField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunTestSettings
    {

        private TestRunTestSettingsDeployment deploymentField;

        private string nameField;

        private string idField;

        /// <remarks/>
        public TestRunTestSettingsDeployment Deployment
        {
            get
            {
                return this.deploymentField;
            }
            set
            {
                this.deploymentField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunTestSettingsDeployment
    {

        private string runDeploymentRootField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string runDeploymentRoot
        {
            get
            {
                return this.runDeploymentRootField;
            }
            set
            {
                this.runDeploymentRootField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunUnitTestResult
    {

        private TestRunUnitTestResultOutput outputField;

        private string executionIdField;

        private string testIdField;

        private string testNameField;

        private string computerNameField;

        private System.DateTime durationField;

        private string testTypeField;

        private string outcomeField;

        private string testListIdField;

        private string relativeResultsDirectoryField;

        /// <remarks/>
        public TestRunUnitTestResultOutput Output
        {
            get
            {
                return this.outputField;
            }
            set
            {
                this.outputField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string executionId
        {
            get
            {
                return this.executionIdField;
            }
            set
            {
                this.executionIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string testId
        {
            get
            {
                return this.testIdField;
            }
            set
            {
                this.testIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string testName
        {
            get
            {
                return this.testNameField;
            }
            set
            {
                this.testNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string computerName
        {
            get
            {
                return this.computerNameField;
            }
            set
            {
                this.computerNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "time")]
        public System.DateTime duration
        {
            get
            {
                return this.durationField;
            }
            set
            {
                this.durationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string testType
        {
            get
            {
                return this.testTypeField;
            }
            set
            {
                this.testTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string outcome
        {
            get
            {
                return this.outcomeField;
            }
            set
            {
                this.outcomeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string testListId
        {
            get
            {
                return this.testListIdField;
            }
            set
            {
                this.testListIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string relativeResultsDirectory
        {
            get
            {
                return this.relativeResultsDirectoryField;
            }
            set
            {
                this.relativeResultsDirectoryField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunUnitTestResultOutput
    {

        private TestRunUnitTestResultOutputErrorInfo errorInfoField;

        /// <remarks/>
        public TestRunUnitTestResultOutputErrorInfo ErrorInfo
        {
            get
            {
                return this.errorInfoField;
            }
            set
            {
                this.errorInfoField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunUnitTestResultOutputErrorInfo
    {

        private string messageField;

        /// <remarks/>
        public string Message
        {
            get
            {
                return this.messageField;
            }
            set
            {
                this.messageField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunUnitTest
    {

        private TestRunUnitTestExecution executionField;

        private TestRunUnitTestTestMethod testMethodField;

        private string nameField;

        private string storageField;

        private string idField;

        /// <remarks/>
        public TestRunUnitTestExecution Execution
        {
            get
            {
                return this.executionField;
            }
            set
            {
                this.executionField = value;
            }
        }

        /// <remarks/>
        public TestRunUnitTestTestMethod TestMethod
        {
            get
            {
                return this.testMethodField;
            }
            set
            {
                this.testMethodField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string storage
        {
            get
            {
                return this.storageField;
            }
            set
            {
                this.storageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunUnitTestExecution
    {

        private string idField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunUnitTestTestMethod
    {

        private string codeBaseField;

        private string adapterTypeNameField;

        private string classNameField;

        private string nameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string codeBase
        {
            get
            {
                return this.codeBaseField;
            }
            set
            {
                this.codeBaseField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string adapterTypeName
        {
            get
            {
                return this.adapterTypeNameField;
            }
            set
            {
                this.adapterTypeNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string className
        {
            get
            {
                return this.classNameField;
            }
            set
            {
                this.classNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunTestEntry
    {

        private string testIdField;

        private string executionIdField;

        private string testListIdField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string testId
        {
            get
            {
                return this.testIdField;
            }
            set
            {
                this.testIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string executionId
        {
            get
            {
                return this.executionIdField;
            }
            set
            {
                this.executionIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string testListId
        {
            get
            {
                return this.testListIdField;
            }
            set
            {
                this.testListIdField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunTestList
    {

        private string nameField;

        private string idField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunResultSummary
    {

        private TestRunResultSummaryCounters countersField;

        private TestRunResultSummaryOutput outputField;

        private TestRunResultSummaryRunInfo[] runInfosField;

        private string outcomeField;

        /// <remarks/>
        public TestRunResultSummaryCounters Counters
        {
            get
            {
                return this.countersField;
            }
            set
            {
                this.countersField = value;
            }
        }

        /// <remarks/>
        public TestRunResultSummaryOutput Output
        {
            get
            {
                return this.outputField;
            }
            set
            {
                this.outputField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("RunInfo", IsNullable = false)]
        public TestRunResultSummaryRunInfo[] RunInfos
        {
            get
            {
                return this.runInfosField;
            }
            set
            {
                this.runInfosField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string outcome
        {
            get
            {
                return this.outcomeField;
            }
            set
            {
                this.outcomeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunResultSummaryCounters
    {

        private byte totalField;

        private byte executedField;

        private byte passedField;

        private byte failedField;

        private byte errorField;

        private byte timeoutField;

        private byte abortedField;

        private byte inconclusiveField;

        private byte passedButRunAbortedField;

        private byte notRunnableField;

        private byte notExecutedField;

        private byte disconnectedField;

        private byte warningField;

        private byte completedField;

        private byte inProgressField;

        private byte pendingField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte total
        {
            get
            {
                return this.totalField;
            }
            set
            {
                this.totalField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte executed
        {
            get
            {
                return this.executedField;
            }
            set
            {
                this.executedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte passed
        {
            get
            {
                return this.passedField;
            }
            set
            {
                this.passedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte failed
        {
            get
            {
                return this.failedField;
            }
            set
            {
                this.failedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte error
        {
            get
            {
                return this.errorField;
            }
            set
            {
                this.errorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte timeout
        {
            get
            {
                return this.timeoutField;
            }
            set
            {
                this.timeoutField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte aborted
        {
            get
            {
                return this.abortedField;
            }
            set
            {
                this.abortedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte inconclusive
        {
            get
            {
                return this.inconclusiveField;
            }
            set
            {
                this.inconclusiveField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte passedButRunAborted
        {
            get
            {
                return this.passedButRunAbortedField;
            }
            set
            {
                this.passedButRunAbortedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte notRunnable
        {
            get
            {
                return this.notRunnableField;
            }
            set
            {
                this.notRunnableField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte notExecuted
        {
            get
            {
                return this.notExecutedField;
            }
            set
            {
                this.notExecutedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte disconnected
        {
            get
            {
                return this.disconnectedField;
            }
            set
            {
                this.disconnectedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte warning
        {
            get
            {
                return this.warningField;
            }
            set
            {
                this.warningField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte completed
        {
            get
            {
                return this.completedField;
            }
            set
            {
                this.completedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte inProgress
        {
            get
            {
                return this.inProgressField;
            }
            set
            {
                this.inProgressField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte pending
        {
            get
            {
                return this.pendingField;
            }
            set
            {
                this.pendingField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunResultSummaryOutput
    {

        private string stdOutField;

        /// <remarks/>
        public string StdOut
        {
            get
            {
                return this.stdOutField;
            }
            set
            {
                this.stdOutField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public partial class TestRunResultSummaryRunInfo
    {

        private string textField;

        private string computerNameField;

        private string outcomeField;

        private System.DateTime timestampField;

        /// <remarks/>
        public string Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string computerName
        {
            get
            {
                return this.computerNameField;
            }
            set
            {
                this.computerNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string outcome
        {
            get
            {
                return this.outcomeField;
            }
            set
            {
                this.outcomeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime timestamp
        {
            get
            {
                return this.timestampField;
            }
            set
            {
                this.timestampField = value;
            }
        }
    }
}
