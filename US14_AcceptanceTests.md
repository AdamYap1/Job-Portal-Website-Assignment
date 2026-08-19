# US-14 Acceptance Test Cases — Manual Test Procedures

## Test Setup

### Test Data Requirements

#### Job Seeker A
- Email: seeker.a@test.com
- Password: TestPass123
- Applications:
  - Application 1: Software Engineer (ABC Corp) - Applied: 01/08/2026 - Status: Pending (Submitted)
  - Application 2: Web Developer (XYZ Corp) - Applied: 02/08/2026 - Status: Accepted
  - Application 3: Backend Developer (DEF Corp) - Applied: 03/08/2026 - Status: Rejected

#### Job Seeker B
- Email: seeker.b@test.com
- Password: TestPass123
- Applications:
  - Application 4: Frontend Engineer (ABC Corp) - Applied: 04/08/2026 - Status: Pending (Submitted)

#### Employer A
- Email: employer.a@test.com
- Password: TestPass123
- Company: ABC Corp
- Jobs: Software Engineer, Frontend Engineer

#### Employer B
- Email: employer.b@test.com
- Password: TestPass123
- Company: XYZ Corp
- Jobs: Web Developer

#### Employer C
- Email: employer.c@test.com
- Password: TestPass123
- Company: DEF Corp
- Jobs: Backend Developer

---

## Acceptance Test 1: Application Details Displayed Correctly

**Given:** I am logged in as Job Seeker A  
**When:** I open the My Applications page  
**Then:** I should see my submitted application  

**Steps:**
1. Log out (if logged in)
2. Click "Login"
3. Enter seeker.a@test.com / TestPass123
4. Navigate to My Applications page
5. Verify Application 1 is displayed

**Expected Results:**
- [ ] Application displays Job Title: "Software Engineer"
- [ ] Application displays Company Name: "ABC Corp"
- [ ] Application displays Application Date: "01 Aug 2026"
- [ ] Application displays Status: "Pending"

**Test Result:** ☐ Pass ☐ Fail

---

## Acceptance Test 2: Multiple Applications Displayed

**Given:** I (Job Seeker A) have submitted multiple job applications  
**When:** I open the My Applications page  
**Then:** All submitted applications are displayed  

**Steps:**
1. Log in as Job Seeker A
2. Open My Applications page
3. Count displayed applications
4. Verify each application is independent

**Expected Results:**
- [ ] Application 1 visible: Software Engineer | ABC Corp | 01 Aug 2026 | Pending
- [ ] Application 2 visible: Web Developer | XYZ Corp | 02 Aug 2026 | Accepted
- [ ] Application 3 visible: Backend Developer | DEF Corp | 03 Aug 2026 | Rejected
- [ ] Total 3 applications displayed
- [ ] Each has independent status

**Test Result:** ☐ Pass ☐ Fail

---

## Acceptance Test 3: Application Status Updates Correctly

**Given:** My application has been reviewed by the employer  
**When:** I refresh or revisit the My Applications page  
**Then:** The updated application status is displayed  

**Steps:**
1. Log in as Employer A
2. Find Application 1 (Job Seeker A's Software Engineer application)
3. Update status from "Pending" (Submitted) to "Accepted"
4. Save changes
5. Log out
6. Log in as Job Seeker A
7. Open My Applications page
8. Refresh the page (F5)

**Expected Results:**
- [ ] Application 1 now displays Status: "Accepted" (not "Pending")
- [ ] Other applications unchanged
- [ ] No page cache issues; fresh data from database

**Test Repeat for Rejected Status:**
1. Log in as Employer A
2. Update Application 1 to "Rejected"
3. Log in as Job Seeker A
4. Refresh My Applications

**Expected Results:**
- [ ] Application 1 displays Status: "Rejected"

**Test Result:** ☐ Pass ☐ Fail

---

## Acceptance Test 4: No Applications Found

**Given:** I have not submitted any job applications  
**When:** I open the My Applications page  
**Then:** No application records are displayed  

**Steps:**
1. Create a new Job Seeker account: seeker.c@test.com / TestPass123
2. Log in with this account
3. Open My Applications page

**Expected Results:**
- [ ] No applications table displayed
- [ ] Message displays exactly: "You have not submitted any job applications yet."
- [ ] Message is clearly visible

**Test Result:** ☐ Pass ☐ Fail

---

## Acceptance Test 5: Only Logged-in User's Applications Displayed

**Given:** Multiple job seekers have submitted job applications  
**When:** I log in as Job Seeker A and open My Applications page  
**Then:** Only my own applications are displayed  

**Steps:**
1. Log in as Job Seeker A
2. Open My Applications page
3. Verify all Job Seeker A's applications
4. Log out
5. Log in as Job Seeker B
6. Open My Applications page
7. Verify only Job Seeker B's applications

**Expected Results - Job Seeker A:**
- [ ] Applications 1, 2, 3 visible
- [ ] Application 4 NOT visible

**Expected Results - Job Seeker B:**
- [ ] Application 4 visible
- [ ] Applications 1, 2, 3 NOT visible

**Test Result:** ☐ Pass ☐ Fail

---

## Acceptance Test 6: Guest Users Cannot Access Application Status

**Given:** I am not logged in  
**When:** I attempt to access the My Applications page directly  
**Then:** I am redirected to the Login page  

**Steps:**
1. Open browser's Developer Tools → Storage → Cookies
2. Delete all cookies (clear session)
3. Navigate directly to: /Applications/MyApplications
4. Observe the page

**Expected Results:**
- [ ] Page does not load the My Applications content
- [ ] User is redirected to /Account/Login
- [ ] Login form is displayed

**Test Result:** ☐ Pass ☐ Fail

---

## Acceptance Test 7: Employer Cannot Access Job Seeker Application Status

**Given:** I am logged in as an Employer  
**When:** I attempt to access the My Applications page  
**Then:** "access is denied" message is displayed  

**Steps:**
1. Log out (if logged in)
2. Log in as Employer A (employer.a@test.com / TestPass123)
3. Attempt to navigate to /Applications/MyApplications

**Expected Results:**
- [ ] Page displays: "access is denied"
- [ ] My Applications content is NOT displayed
- [ ] User is not authenticated as JobSeeker role

**Test Result:** ☐ Pass ☐ Fail

---

## Test Execution Summary

| Test # | Test Name | Result | Notes |
|--------|-----------|--------|-------|
| 1 | Application Details | ☐ Pass ☐ Fail | |
| 2 | Multiple Applications | ☐ Pass ☐ Fail | |
| 3 | Status Updates | ☐ Pass ☐ Fail | |
| 4 | Empty State | ☐ Pass ☐ Fail | |
| 5 | User Isolation | ☐ Pass ☐ Fail | |
| 6 | Guest Access | ☐ Pass ☐ Fail | |
| 7 | Employer Access | ☐ Pass ☐ Fail | |

---

## Sign-Off

- Tested By: ___________________
- Date: ___________________
- All Tests Passed: ☐ Yes ☐ No
