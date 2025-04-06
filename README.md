# FMInatorul.Tests

This directory contains the automated tests for the FMInatorul project. These tests are designed to verify the functionality, security, and behavior of the application's core features, focusing on student and professor workflows, security measures, and critical functions such as PDF uploads and quiz generation. Each test category is thoroughly described to ensure clarity for developers, testers, and maintainers.

---

# Test Categories and Purpose

The tests are organized into three main categories: StudentsAndProfessorsFlowTests (Integration Tests), HomeControllerSecurityTests (Security Tests), ProfessorsControllerSecurityTests (Security Tests), RoomsControllerSecurityTests (Security Tests), StudentsControllerSecurityTests (Security Tests), and CriticalFunctionsUnitTests (Unit Tests). Each category has a specific purpose in ensuring the application's reliability and security.

## 1. Integration Tests

### StudentsAndProfessorsFlowTests.cs

These tests focus explicitly on the integration and workflow interactions between students and professors in the FMInatorul application. They ensure that the controllers, user roles, and database operations work correctly together in real-world scenarios, testing end-to-end functionality.

- *Student_UploadPdf_ShouldSaveFileAndGenerateQuiz*  
  Uses a mocked IFormFile object simulating a PDF file named "test.pdf" with a valid size (1024 bytes) and content type ("application/pdf"). The test verifies that the result of the UploadPdf action in the StudentsController is an ObjectResult, confirming that the file is saved and a quiz is generated. Ensures the core feature of uploading educational materials (e.g., PDFs) and converting them into quizzes for students functions as expected without errors.

- *EditCollegeProf_UserIsStudent_ReturnsForbidResult*  
  Checks that a student user cannot modify a professor's data. The test creates entities for a college, student, professor, subject, question, and answer option, then attempts to edit a professor's data using a student account. The expected outcome is a ForbidResult, indicating that access is denied.

---

## 2. Unit Tests

### CriticalFunctionsUnitTests.cs

These unit tests explicitly focus on the critical functionality of the StudentsController, specifically the UploadPdf action. They ensure that uploading a PDF results in a valid quiz with questions, answers, and correct options, testing the logic in isolation.

- *UploadPdf_QuizNotNullAsync*  
  Ensures that the UploadPdf method does not return a Bad Request response when a valid PDF file is used.
  Uses a mocked IFormFile with sample PDF content ("Sample PDF Content") and checks that the result of the UploadPdf action is not a BadRequestObjectResult, ensuring a valid quiz is generated.

- *UploadPdf_QuizHasQuestionsAsync*  
  Ensures that the generated quiz contains questions.
  Checks that the QuizModel returned in a ViewResult has a non-empty list of questions. Handles potential errors (e.g., invalid file formats) by verifying error messages like "Invalid file format. Only PDF files allowed!".

- *UploadPdf_QuestionsHaveText*  
  For each question in the quiz, this test ensures that the question text is not null or composed solely of whitespace.

- *UploadPdf_QuestionsHaveAnswers*  
  Verifies that each quiz question contains at least one valid option (choice), ensuring that the list of answer options is neither null nor empty.

- *UploadPdf_QuestionsHaveCorrectAnswers*  
  Ensures that each quiz question has an associated correct answer by verifying that the answer property is not null.

---

## 3. Security Tests

The security tests ensure that the application's controllers enforce proper authentication, authorization, and access control. They verify that only authorized users can perform actions and that unauthorized access is correctly denied with appropriate HTTP status codes (e.g., 401 Unauthorized). The following controllers are tested for security: HomeController, ProfessorsController, RoomsController, and StudentsController.

### HomeControllerSecurityTests.cs

These tests explicitly focus on the security aspects of the HomeController, ensuring that public actions (e.g., Index, Privacy) are accessible, admin-only actions (e.g., Admin) are restricted, and file uploads are secure. They also test error handling and role-based access.

- *Index, IndexNew, and Privacy*  
  These tests verify that the Index, IndexNew, and Privacy actions correctly return a ViewResult without specifying a view name, meaning that the default view is used.

- *Error_ReturnsViewResult_WithErrorDetails*  
  Sets up a mock user context with a specific user ID ("test-user-id") and verifies that the ErrorViewModel in the result contains a non-null RequestId matching the mock trace identifier ("test-trace-id").

- *Admin_ReturnsRedirect_WhenUserIsNotAdmin & Admin_ReturnsView_WhenUserIsAdmin*
  Uses mock users with and without admin roles. The non-admin test expects a RedirectToActionResult to "Index", while the admin test sets up mock subject data (e.g., "Math") and verifies a ViewResult with the data in ViewData["materii"].
  Tests the behavior of the Admin action:
  - If the user is not an administrator, a redirection to the Index action is expected.
  - If the user is an administrator, a ViewResult is returned with the relevant data (e.g., the list of subjects).

- *Add_Questions_FileUpload_ReturnsBadRequest_WhenFileIsInvalid & Add_Questions_FileUpload_ReturnsSuccess_WhenFileIsPdf*  
  Verifies the file upload logic in the Add_Questions action:
  - In the case of an invalid file type (e.g., ZIP), a Bad Request response is returned.
  - For a valid PDF file, the workflow continues correctly, returning either an Unauthorized status (if applicable) or a redirection to the Admin page.

---

### ProfessorsControllerSecurityTests.cs

These tests explicitly focus on the security of the ProfessorsController, ensuring that only authorized professors (or users with appropriate roles) can access actions like viewing the index, editing subjects, validating questions, or invalidating questions. Unauthorized users should be denied access with a 401 Unauthorized response.

- *Index_ShouldReturn401_WhenUserIsUnauthorized*  
  Checks that the Index method in the professors' controller returns a ChallengeResult when the user is not authenticated.

- *EditMaterie_ShouldReturn401_WhenUserIsUnauthorized*  
  Tests that access to the EditMaterie method is restricted for unauthorized users, returning a ChallengeResult.

- *EditMaterie_WithProfessor_ShouldReturn401_WhenUserIsUnauthorized*  
  Creates entities for a college, users, student, and professor, as well as subject and questions, and verifies that the EditMaterie method (called with a professor object) returns a ChallengeResult when the user is not authenticated.

- *Valideaza_ShouldReturn401_WhenUserIsUnauthorized*  
  Checks that access to the Valideaza method (used for validating questions) is blocked for unauthorized users.

- *NuValideaza_ShouldReturn401_WhenUserIsUnauthorized*  
  Similarly, ensures that the NuValideaza method (used for invalidating questions) returns a ChallengeResult for unauthorized access.

---

### RoomsControllerSecurityTests.cs

These tests explicitly verify the security of the RoomsController, which handles room-related actions such as joining and leaving rooms. They ensure that only authenticated users can interact with rooms and that invalid or unauthorized actions (e.g., joining non-existent rooms) are handled correctly.

- *JoinRoom_ShouldReturnUnauthorized_WhenUserIsNotLoggedIn*  
  Verifies that an unauthenticated user receives a JSON response indicating that login is required to join a room.
  Mocks the UserManager to return null for the user ID and checks that the JoinRoom action returns a JsonResult with the expected error message serialized in JSON.

- *JoinRoom_ShouldReturnSuccess_WhenUserIsAuthorized*  
  Explicitly verifies that an authenticated user can successfully join a room, returning a success message in JSON format.
  Sets up test data (faculty, users, students, professors, subjects, rooms) and mocks an authorized user (e.g., a student). Checks that the JoinRoom action returns a JsonResult with success = true and a message like "Joined room {roomCode} successfully."

- *JoinRoom_ShouldReturnNotFound_WhenRoomDoesNotExist*
  Explicitly tests that attempting to join a non-existent room results in a "Room not found" error in the JSON response.
  Uses test data but provides an invalid room code (e.g., "999999"). Verifies the JsonResult contains success = false and the message "Room not found".

- *LeaveRoom_ShouldReturnSuccess_WhenUserIsInRoom*  
  Tests the functionality of leaving a room for a user who is already a member, verifying that the operation is successful.

- *LeaveRoom_ShouldReturnError_WhenUserIsNotInRoom*  
  Checks that if a user attempts to leave a room they are not part of, a JSON response is returned indicating the error (e.g., "Room not found").

---

### StudentsControllerSecurityTests.cs

These tests explicitly focus on the security of the StudentsController, ensuring that only authenticated students (or authorized users) can access actions like viewing the index, playing quizzes, uploading PDFs, or editing student data. Unauthorized users should be denied access with a 401 Unauthorized response.

- *Index_ShouldReturn401_WhenUserIsUnauthorized*  
  Tests access to the Index action, verifying that an unauthenticated user receives a ChallengeResult.

- *Play_ShouldReturn401_WhenUserIsUnauthorized*  
  Verifies that the Play action (used to start a game session or quiz) returns a ChallengeResult for unauthorized users.

- *UploadPdf (GET) - UploadPdf_ShouldReturn401_WhenUserIsNotAauthorized*  
  Ensures that the GET request for the UploadPdf action is restricted and returns a ChallengeResult if the user is not authenticated.

- *UploadPdf (POST) - UploadPdf_ShouldReturn401_WhenAuthenticationFails*  
  Tests the scenario where, during the PDF upload, authentication fails (for example, when the JWT token is empty). It checks that an error response (status 401) with a specific message is returned.

- *ChatView_ShouldReturn401_WhenUserIsUnauthorized*  
  Verifies that access to the ChatView action is restricted for unauthorized users.

- *MateriiSingle_ShouldReturn401_WhenUserIsUnauthorized*  
  Tests access to the MateriiSingle action, ensuring that only authenticated users can access detailed subject information.

- *EditYear_ShouldReturn401_WhenUserIsUnauthorized*  
  Ensures that the EditYear method (for editing the study year) is not accessible to unauthorized users.

- *EditSemester_ShouldReturn401_WhenUserIsUnauthorized*  
  Similar to the previous test, but for the EditSemester action, which modifies the study semester.

- *EditCollege_ShouldReturn401_WhenUserIsUnauthorized*  
  Checks that access to the college (faculty) editing action is protected and returns a ChallengeResult for unauthenticated users.
  Mocks a PDF file upload and overrides a static helper method (HttpHelper.GetJwtTokenAsync) to return an empty token, simulating authentication failure. Verifies that the result is an ObjectResult with a status code of 401 and an error message like "Failed to authenticate with the Flask API".

---
## Common Patterns and Tools

- *Mocking*
Mocking: All tests explicitly use the Moq library to create mock objects for dependencies such as UserManager, RoleManager, IFormFile, HttpContext, ILogger, and IHubContext. This isolates the code being tested from external systems, ensuring tests are fast and reliable.

- *xUnit*
xUnit: The tests are explicitly written using the xUnit testing framework, with [Fact] attributes to denote individual test methods, ensuring each test is independent and self-contained.

- *Async*
Async Testing: Many tests are explicitly asynchronous (async Task) to handle operations like file uploads, database interactions, and API calls, reflecting real-world application behavior.

---

## Conclusion

The tests in the *FMInatorul.Tests* directory ensure the quality and security of critical functionalities in the FMInatorul application by covering:

- *Integrated workflows* and unit functions (such as PDF file upload and quiz generation) within the student and professor controllers.
- *Security aspects* for all main controllers (Home, Professors, Rooms, Students), ensuring that only authenticated and authorized users can access critical operations.
- The protection of operations related to managing rooms, chat functionality, and other sensitive data flows by returning a ChallengeResult (equivalent to a 401 Unauthorized status) when access is not permitted.

These tests are essential for maintaining code stability and for the early detection of issues before the application reaches production.
