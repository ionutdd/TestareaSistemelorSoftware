# FMInatorul.Tests

This directory contains the automated tests for the FMInatorul project. The tests are divided into two main categories: Integration/Unit Tests and Security Tests. Each category aims to verify different aspects of the application's functionality and security.

---

## 1. Integration & Unit Tests

### StudentsAndProfessorsFlowTests.cs

This file contains tests that verify workflows involving interactions between students and professors, as well as the PDF file upload functionality and quiz generation.

- *Student_UploadPdf_ShouldSaveFileAndGenerateQuiz*  
  Tests the method that handles the PDF file upload in the student controller. It uses a valid PDF file (correct size and type) to ensure that the method correctly processes the file and generates a quiz object.

- *EditCollegeProf_UserIsStudent_ReturnsForbidResult*  
  Checks that a student user cannot modify a professor's data. The test creates entities for a college, student, professor, subject, question, and answer option, then attempts to edit a professor's data using a student account. The expected outcome is a ForbidResult, indicating that access is denied.

---

### CriticalFunctionsUnitTests.cs

This file contains unit tests for critical functions in the student controller, focusing on the UploadPdf method. The goal is to validate that the PDF upload process correctly generates a quiz, and that the resulting quiz contains valid questions and answers.

- *UploadPdf_QuizNotNullAsync*  
  Ensures that the UploadPdf method does not return a Bad Request response when a valid PDF file is used.

- *UploadPdf_QuizHasQuestionsAsync*  
  Confirms that the generated quiz object contains questions and that the list of questions is not empty. It also handles the possibility of an error response (Bad Request) in case of an invalid file.

- *UploadPdf_QuestionsHaveText*  
  For each question in the quiz, this test ensures that the question text is not null or composed solely of whitespace.

- *UploadPdf_QuestionsHaveAnswers*  
  Verifies that each quiz question contains at least one valid option (choice), ensuring that the list of answer options is neither null nor empty.

- *UploadPdf_QuestionsHaveCorrectAnswers*  
  Ensures that each quiz question has an associated correct answer by verifying that the answer property is not null.

---

## 2. Security Tests

The security tests ensure that access to critical actions in the application controllers is properly restricted and that unauthorized users receive a "Challenge" response (401 Unauthorized).

### HomeControllerSecurityTests.cs

This file focuses on testing the security aspects of the main controller (HomeController), ensuring that the application adheres to authentication and authorization rules.

- *Index, IndexNew, and Privacy*  
  These tests verify that the Index, IndexNew, and Privacy actions correctly return a ViewResult without specifying a view name, meaning that the default view is used.

- *Error_ReturnsViewResult_WithErrorDetails*  
  Ensures that when the Error action is called, it returns a view containing error details (e.g., RequestId) and that the trace identifier is correctly set.

- *Admin_ReturnsRedirect_WhenUserIsNotAdmin & Admin_ReturnsView_WhenUserIsAdmin*  
  Tests the behavior of the Admin action:
  - If the user is not an administrator, a redirection to the Index action is expected.
  - If the user is an administrator, a ViewResult is returned with the relevant data (e.g., the list of subjects).

- *Add_Questions_FileUpload_ReturnsBadRequest_WhenFileIsInvalid & Add_Questions_FileUpload_ReturnsSuccess_WhenFileIsPdf*  
  Verifies the file upload logic in the Add_Questions action:
  - In the case of an invalid file type (e.g., ZIP), a Bad Request response is returned.
  - For a valid PDF file, the workflow continues correctly, returning either an Unauthorized status (if applicable) or a redirection to the Admin page.

---

### ProfessorsControllerSecurityTests.cs

This file tests the security of actions in the professors' controller.

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

This file tests the security for managing "rooms" in the application.

- *JoinRoom_ShouldReturnUnauthorized_WhenUserIsNotLoggedIn*  
  Verifies that an unauthenticated user receives a JSON response indicating that login is required to join a room.

- *JoinRoom_ShouldReturnSuccess_WhenUserIsAuthorized*  
  Tests the scenario where an authorized user can successfully join a room. The database is seeded with relevant entities (college, users, subject, etc.), and the response confirms a successful operation.

- *JoinRoom_ShouldReturnNotFound_WhenRoomDoesNotExist*  
  Ensures that if a user tries to join a non-existent room, a JSON response is returned with the message "Room not found."

- *LeaveRoom_ShouldReturnSuccess_WhenUserIsInRoom*  
  Tests the functionality of leaving a room for a user who is already a member, verifying that the operation is successful.

- *LeaveRoom_ShouldReturnError_WhenUserIsNotInRoom*  
  Checks that if a user attempts to leave a room they are not part of, a JSON response is returned indicating the error (e.g., "Room not found").

---

### StudentsControllerSecurityTests.cs

This file contains security tests for the actions in the students' controller, protecting operations that require authentication.

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

---

## Conclusion

The tests in the *FMInatorul.Tests* directory ensure the quality and security of critical functionalities in the FMInatorul application by covering:

- *Integrated workflows* and unit functions (such as PDF file upload and quiz generation) within the student and professor controllers.
- *Security aspects* for all main controllers (Home, Professors, Rooms, Students), ensuring that only authenticated and authorized users can access critical operations.
- The protection of operations related to managing rooms, chat functionality, and other sensitive data flows by returning a ChallengeResult (equivalent to a 401 Unauthorized status) when access is not permitted.

These tests are essential for maintaining code stability and for the early detection of issues before the application reaches production.
