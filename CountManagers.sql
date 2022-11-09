SELECT Departments.Name, COUNT(*) AS Kolich FROM Managers a LEFT JOIN Departments ON Departments.Id = a.Id_main_dep GROUP BY Departments.Name

--SELECT * FROM Departments
--SELECT * FROM Managers
--ORDER BY Id_main_dep

--LEFT JOIN  ON ts.Id = a.Id_main_dep

