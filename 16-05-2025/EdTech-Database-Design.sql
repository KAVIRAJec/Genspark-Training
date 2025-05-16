--Phase 1(schema preparation)
-- Tables:
-- Student
-- Trainer
-- Course (->Trainer)(root)
-- Enrollment(->course & student)
-- Certification(->course & student)

-- Table Design
-- Address (address_id, address, city_id)
-- City (city_id, city,postal_code,district_id)
-- District (district_id,district,country_id)
-- Country (country_id,country)
-- Student (student_id,first_name,last_name,email,contact,address_id,created_at)
-- Trainer (trainer_id,first_name,last_name,email,contact,address_id,experince,created_at)
-- ....

-- Security & Best Practices:
-- 1) Transactions
-- 2) Roles

-- Table design:
CREATE TABLE students (
	student_id SERIAL PRIMARY KEY,
	name VARCHAR(50) NOT NULL,
	email VARCHAR(50) NOT NULL,
	phone VARCHAR(12) NOT NULL,
	created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
CREATE TABLE courses (
	course_id SERIAL PRIMARY KEY,
	course_name VARCHAR(50) NOT NULL,
	category VARCHAR(30),
	duration_days INT,
	created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
ALTER SEQUENCE courses_course_id_seq RESTART WITH 100;

CREATE TABLE trainers (
	trainer_id SERIAL PRIMARY KEY,
	trainer_name VARCHAR(50) NOT NULL,
	expertise VARCHAR(20),
	created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
ALTER SEQUENCE trainers_trainer_id_seq RESTART WITH 200;

CREATE TABLE enrollments (
	enrollment_id SERIAL PRIMARY KEY,
	student_id INT NOT NULL,
	course_id INT NOT NULL,
	enroll_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	FOREIGN KEY (student_id) REFERENCES students(student_id) ON DELETE CASCADE,
	FOREIGN KEY (course_id) REFERENCES courses(course_id) ON DELETE CASCADE
);
ALTER SEQUENCE enrollments_enrollment_id_seq RESTART WITH 300;

CREATE TABLE certificates(
	certificate_id SERIAL PRIMARY KEY,
	enrollment_id INT,
	issue_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	serial_no TEXT unique,
	FOREIGN KEY (enrollment_id) REFERENCES enrollments(enrollment_id) ON DELETE SET NULL
);
ALTER SEQUENCE certificates_certificate_id_seq RESTART WITH 400;

CREATE TABLE course_trainer(
	course_id INT,
	trainer_id INT,
	PRIMARY KEY(course_id,trainer_id),
	FOREIGN KEY (course_id) REFERENCES courses(course_id) ON DELETE CASCADE,
    FOREIGN KEY (trainer_id) REFERENCES trainers(trainer_id) ON DELETE CASCADE
);


-- Phase 2: DDL & DML
-- Insert into students
INSERT INTO students (name, email, phone) VALUES
('Alice Smith', 'alice@example.com', '1234567890'),
('Bob Johnson', 'bob@example.com', '0987654321');

-- Insert into trainers
INSERT INTO trainers ( trainer_name, expertise) VALUES
('Dr. Jane Doe', 'Data Science'),
('Mr. John Roe', 'Cybersecurity');

-- Insert into courses
INSERT INTO courses (course_name, category, duration_days) VALUES
('Python Programming', 'Programming', 30),
('Network Security', 'Security', 45);

-- Insert into course_trainer
INSERT INTO course_trainer (course_id, trainer_id) VALUES
(100, 200),
(101, 201);

-- Insert into enrollments
INSERT INTO enrollments (student_id, course_id) VALUES
(1, 100),
(2, 101);

-- Insert into certificates
INSERT INTO certificates (enrollment_id, serial_no) VALUES
(302, 'CERT-ALICE-101'),
(303, 'CERT-BOB-102');

-- Creating an index:
CREATE INDEX idx_enrollments_student_id ON enrollments(student_id);
CREATE INDEX idx_students_name ON students(name);
CREATE INDEX idx_trainers_name ON trainers(trainer_name);
CREATE INDEX idx_enrollments_course_id ON enrollments(course_id);

-- Phase 3: SQL JOINS Practice
-- 1) List students and the courses they enrolled in
SELECT s.name Student_Name, c.course_name
FROM students s JOIN enrollments e on s.student_id =e.student_id
JOIN courses c on c.course_id = e.course_id

-- 2) Show students who received certificates with trainer names
SELECT s.name Student_Name, t.trainer_name, c.serial_no
FROM students s JOIN enrollments e on s.student_id =e.student_id
JOIN certificates c on c.enrollment_id = e.enrollment_id
JOIN course_trainer ct on ct.course_id = e.course_id
JOIN trainers t on t.trainer_id = ct.trainer_id

-- 3) Count number of students per course
SELECT c.course_name, count(e.student_id) student_per_course
from courses c JOIN enrollments e ON e.course_id = c.course_id
GROUP BY c.course_name

-- Phase 4: Functions & Stored Procedures
-- 1) Returns a list of students who completed the given course and received certificates.
CREATE OR REPLACE FUNCTION get_certified_students(p_course_id INT)
RETURNS TABLE (
	student_id INT,
	student_name VARCHAR,
	Serial_ID TEXT,
	course_name VARCHAR
) AS $$
BEGIN
	RETURN QUERY
	SELECT s.student_id,s.name student_name, c.serial_no Serial_ID, cr.course_name
	FROM students s JOIN enrollments e on s.student_id = e.student_id
	JOIN courses cr ON e.course_id = cr.course_id
	JOIN certificates c on e.enrollment_id = c.enrollment_id
	WHERE e.course_id = p_course_id;
END;
$$ LANGUAGE plpgsql;

SELECT * FROM get_certified_students(101);

-- 2) Inserts into `enrollments` and conditionally adds a certificate if completed (simulate with status flag).
CREATE OR REPLACE PROCEDURE sp_enroll_student(p_student_id INT, p_course_id INT,p_status INT)
AS $$
DECLARE
    v_enrollment_id INT;
begin
	BEGIN
		INSERT INTO enrollments (student_id, course_id) 
		VALUES (p_student_id, p_course_id)
		RETURNING enrollment_id into v_enrollment_id;
		IF p_status = 1 THEN
			INSERT INTO certificates (enrollment_id, serial_no)
			VALUES (v_enrollment_id, 'CERT-' || p_student_id || '-' || p_course_id);
		END IF;
	EXCEPTION 
		WHEN OTHERS THEN
			Raise NOTICE 'Failed to Enroll student data: %', SQLERRM;
	END;
end;
$$ LANGUAGE plpgsql;

CALL sp_enroll_student(2, 100, 1);

-- Phase 5: Cursor
-- Use a cursor to:
-- * Loop through all students in a course
-- * Print name and email of those who do not yet have certificates
CREATE OR REPLACE PROCEDURE sp_list_uncertified_student(p_course_id INT)
AS $$
DECLARE
	count INT;
	rec record;
	cur cursor for 
		SELECT s.student_id, s.name, s.email, e.enrollment_id
		FROM students s JOIN enrollments e ON s.student_id = e.student_id
		WHERE e.course_id = p_course_id AND s.student_id NOT IN
		(SELECT student_id FROM get_certified_students(p_course_id));
BEGIN
	OPEN cur;
	LOOP
		FETCH cur into rec;
		EXIT WHEN NOT FOUND;

		SELECT COUNT(*) INTO count FROM students WHERE student_id = rec.student_id;

		IF count >=1 THEN
			RAISE NOTICE 'Student_ID: %, Student_Name: %, Student_Email: %, Enrollment_ID: %',
			rec.student_id, rec.name, rec.email, rec.enrollment_id;
			count := 0;
		ELSE
			RAISE NOTICE 'No Student found!';
		END IF;
	END LOOP;
	CLOSE cur;
END
$$ LANGUAGE plpgsql;

CALL sp_list_uncertified_student(101);

-- Phase 6: Security & Roles
-- 1) Create a `readonly_user` role:
--   * Can run `SELECT` on `students`, `courses`, and `certificates`
--   * Cannot `INSERT`, `UPDATE`, or `DELETE`
CREATE ROLE readonly_user LOGIN PASSWORD 'readonly_pass';
GRANT CONNECT ON DATABASE "EdTech DB" TO readonly_user;
GRANT USAGE ON SCHEMA public TO readonly_user;
GRANT SELECT ON students,trainers,courses,enrollments,certificates,course_trainer TO readonly_user;
REVOKE INSERT, UPDATE, DELETE ON students,trainers,courses,enrollments,certificates,course_trainer FROM readonly_user;

-- 2. Create a `data_entry_user` role:
--   * Can `INSERT` into `students`, `enrollments`
--   * Cannot modify certificates directly
CREATE ROLE data_entry_user LOGIN PASSWORD 'data_entry_pass';
GRANT CONNECT ON DATABASE "EdTech DB" TO data_entry_user;
GRANT USAGE ON SCHEMA public TO data_entry_user;
GRANT SELECT ON students,trainers,courses,enrollments,certificates,course_trainer TO data_entry_user;
GRANT INSERT ON students,enrollments TO data_entry_user;
REVOKE UPDATE, DELETE ON students,trainers,courses,enrollments,certificates,course_trainer FROM data_entry_user;
REVOKE ALL ON certificates FROM data_entry_user;

-- Phase 7: Transactions & Atomicity
-- Write a transaction block that:
-- * Enrolls a student
-- * Issues a certificate
-- * Fails if certificate generation fails (rollback)
CREATE OR REPLACE PROCEDURE sp_enroll_Certify(p_student_id INT,p_course_id INT)
AS $$
BEGIN
	BEGIN
		WITH enrollment_insert AS (
		    INSERT INTO enrollments (student_id, course_id)
		    VALUES (p_student_id, p_course_id)
		    RETURNING enrollment_id
		)
		INSERT INTO certificates (enrollment_id, serial_no)
		SELECT enrollment_id, 'CERT-' || p_student_id || '-' || p_course_id
		FROM enrollment_insert;
		RAISE NOTICE 'Data inserted Successfully!';
	EXCEPTION
		WHEN OTHERS THEN
			ROLLBACK;
			RAISE NOTICE 'Failed: %',SQLERRM;
	END;
END;
$$ LANGUAGE plpgsql;

CALL sp_enroll_Certify(3,101);

-- SELECT:
SELECT * FROM students
SELECT * FROM courses
select * from trainers
select * from enrollments
select * from certificates
select * from course_trainer