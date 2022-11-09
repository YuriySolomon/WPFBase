SELECT D.Name, COUNT(M.Name)
FROM Departments D JOIN Managers M ON D.Id = M.Id_main_dep