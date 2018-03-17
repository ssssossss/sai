using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IFNBilling.Domain;
using Rhino.Mocks;
using IFNBilling.ServiceContract;
using IFNBilling.Domain.Model;
using IFNBilling.UI.Controller;
namespace IFNBilling.ControllerTest
{
    /// <summary>
    /// Unit testing on UI ProjectController.
    /// </summary>
    [TestClass]
    public class UT_ProjectController
    {
        #region Memebers
        IProjectContractManager mProjectContractManager;
        #endregion

        #region TestInitialize
        /// <summary>
        /// Tests the initialize.
        /// </summary>
        [TestInitialize]
        public void TestInit()
        {
            mProjectContractManager = MockRepository.GenerateMock<IProjectContractManager>();
        }
        #endregion

        #region TestMethod
        /// <summary>
        /// Gets all project.
        /// </summary>
        [TestMethod]
        public void GetAllProject()
        {
            #region Arrange
            var expected = CreateProjectCollection();
            mProjectContractManager.Stub(p => p.GetAllProjects()).Return(expected);
            #endregion

            #region Act
            var projectController = new ProjectController(mProjectContractManager);
            var actual = projectController.GetAllProjects();
            #endregion

            #region Assert
            Assert.AreEqual(expected.Count, actual.Count);
            #endregion
        }

        /// <summary>
        /// Inserts the update project invoice.
        /// </summary>
        [TestMethod]
        public void InsertUpdateProjectInvoice()
        {
            #region Arrange
            bool expected = true;
            bool actual;
            mProjectContractManager.Expect(mp => mp.InsertUpdateProjectInvoice(null)).IgnoreArguments().Return(true);
            #endregion
            
            #region Act
            var projectController = new ProjectController(mProjectContractManager);
            actual = projectController.InsertUpdateProjectInvoice(null);
            #endregion

            #region Assert
            Assert.AreEqual(expected, actual);
            #endregion
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Creates the project collection.
        /// </summary>
        /// <returns></returns>
        private List<Project> CreateProjectCollection()
        {
            ProjectVertical projectVertical = new ProjectVertical();
            ProjectType projectType = new ProjectType();
            CodeMaster codeMaster = new CodeMaster();
            ProjectStatus projectStatus = new ProjectStatus();
            Contact contact = new Contact();
            contact.ContactID = 0;
            projectType.ProjectTypeID = 0;
            codeMaster.CodeMasterID = 0;
            projectStatus.ProjectStatusID = 0;
            List<Project> projectCollection = new List<Project> 
           {
               new Project()
               { 
               ProjectName = "MockProject"
              ,DistributionList=string.Empty
              ,ProjectVerticalID=0
              ,ProjectVertical=projectVertical
              ,Contact = contact
              ,ProjectNumber =   0
              ,ProjectType = projectType
              ,CodeMaster = codeMaster
              ,InitialEstimatedPages = 0
              ,ExpectedStartDate = DateTime.Now
              ,ProjectStatus = projectStatus
              ,StatusDate = DateTime.Now
              ,IsTypeSet =  true
              ,IsTranslations = true
              ,IsHospitality = true
              ,IsMedia = true
              ,IsFiling = true
              ,IsPrint = true
              ,ExpectedFilingDate = null
              ,Comments = "MockComments"
               },
               new Project()
               { 
               ProjectName = "MockProject1"
              ,DistributionList=string.Empty
              ,ProjectVerticalID=1
              ,ProjectVertical=projectVertical
              ,Contact = contact
              ,ProjectNumber =   1
              ,ProjectType = projectType
              ,CodeMaster = codeMaster
              ,InitialEstimatedPages = 1
              ,ExpectedStartDate = DateTime.Now
              ,ProjectStatus = projectStatus
              ,StatusDate = DateTime.Now
              ,IsTypeSet =  false
              ,IsTranslations = false
              ,IsHospitality = false
              ,IsMedia = false
              ,IsFiling = false
              ,IsPrint = false
              ,ExpectedFilingDate = null
              ,Comments = "MockComments1"
               },

           };

            return projectCollection;
        }
        #endregion
    }
}



