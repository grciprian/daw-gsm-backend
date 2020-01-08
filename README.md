# GsmBackend

## Description :floppy_disk:

  *GSM* reprezinta un proof of concept pentru o platforma prin care un magazin care repara diverse deviceuri isi poate administra *employees*, *customers* si interactiunea dintre acestia pe baza unor *gadgets* si a unor *contracts*.  
  
[## Some screenshots :camera:](https://i.imgur.com/60OuIeI.png)

## How To :bulb:

  By default exista un superadmin care se poate autentifica cu username: superadmin si password: 123qweQWE!@#*  
  Superadminul poate adauga angajati, dar ii poate si sterge. Odata ce un angajat a fost adaugat cu anumite credentiale, acesta se poate autentifica.  
  Nu in ultimul rand, clientii se pot inregistra completand singuri un formular.  
  Interactiunea dintre clienti si angajati are loc astfel:  
  -> un client isi poate adauga in portofoliu gadgets  
  -> tot un client poate pentru un gadget sa creeze un contract prin care va spune ce este in neregula cu dispozitivul  
  -> unui angajat i se atribuie contractul respectiv dupa urmatoarea regula: daca niciun angajat nu are contracte, assignarea se va face aleatoriu, altfel se va alege angajatul cu numarul cel mai mic de contracte  
  -> angajatul vede contractul iar cand i se va aduce gadgetul pentru depanare si reparatie va putea seta data de sfarsit si un status pentru contract  
  -> la randul lui, clientul poate urmarii informatiile din contract pentru care data de final si statusul se pot schimba, astfel stiind cand poate sa ridice bunul personal  

## Observations :bell:

! Nu am tratat validari pe frontend; foarte putin in backend !  
O parola valida trebuie sa contina tot felul de tipuri de caractere, astfel ca una valida ar fi 123qweQWE!@#  
Pentru orice neintelegere, nu evita sa ma contactezi.  

The API server runs on https://localhost:44373
