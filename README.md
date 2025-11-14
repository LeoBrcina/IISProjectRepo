# 📘 IIS Project – Interoperability & Internet Services  
### *(XSD & RNG Validation · REST API · SOAP · JAXB · XML-RPC · WPF Client)*

This repository contains the complete IIS (Interoperability & Internet Services) course project, which includes:

- **InteroperabilityProject** – Main .NET backend  
  - XSD + RNG schema validation  
  - REST API for XML processing  
  - SOAP service with XPath filtering  
  - JSON → XML transformation  
  - SQL Server integration  
  - WPF desktop client

- **IISJaxbValidator** – Java JAXB validator  
- **IISXmlRpcServer** – C# XML-RPC server  

All credentials have been removed.

---

## 📁 Project Structure

IISProjectRepo/
│
├── InteroperabilityProject/          # Main backend (REST, SOAP, DB, WPF)
│   ├── Controllers/                  # REST endpoints (XSD, RNG validation)
│   ├── Interfaces/                   # SOAP interface
│   ├── Services/                     # SOAP implementation
│   ├── XmlSchemas/                   # XSD + RNG schemas
│   ├── XmlData/                      # Saved validated XML files
│   ├── Models/                       # DTOs and domain models
│   ├── Data/                         # SQL Server integration
│   ├── Program.cs
│   └── WPFClient/                    # WPF desktop UI (REST + SOAP)
│
├── IISJaxbValidator/                 # Java JAXB validator
│   ├── Schemas/
│   ├── Data/
│   └── src/
│
└── IISXmlRpcServer/                  # XML-RPC server (C#)
    ├── Models/
    ├── XmlRpcServer.cs
    └── Program.cs

---

## 🎯 Project Overview

### ✔ Step 1 — XSD Validation (REST)
- XML sent via REST  
- Validated using XSD schema  
- Detailed errors returned  
- Valid XML saved to XmlData and database  

### ✔ Step 2 — RNG Validation (REST)
- XML validated using Relax NG schema  
- Errors returned in friendly format  
- Valid files stored  

### ✔ Step 3 — JSON → XML + RapidAPI
- XML contains a LinkedIn profile URL  
- Validated by XSD/RNG  
- Backend calls RapidAPI  
- JSON converted to formatted XML  
- XML saved + relevant fields inserted into SQL database  

### ✔ Step 4 — SOAP Service (XPath Search)
- SOAP loads a combined XML dataset  
- Performs XPath queries  
- Returns matching XML nodes  
- WPF client consumes these results  

### ✔ Step 5 — JAXB Validation (Java)
- Java app validates XML using JAXB-generated classes and XSD  
- Ensures schema compliance  

### ✔ Step 6 — XML-RPC Server (C#)
- Minimal XML-RPC server  
- Exposes simple callable methods  

### ✔ Step 7 — WPF Client
- Sends XML to REST validation endpoints  
- Displays results  
- Sends keywords to SOAP service  
- Shows XPath-filtered results  

---

## 🛠️ Tech Stack

### Backend (.NET)
- ASP.NET WebAPI  
- SOAP  
- SQL Server  
- XML serialization  
- XSD + RNG  
- XPath  

### Java
- JAXB  

### Protocols
- REST  
- SOAP  
- XML-RPC  

### Client
- WPF (C#)

---

## 🚀 Running the Projects

### Backend
cd InteroperabilityProject  
dotnet restore  
dotnet run  

### WPF Client
Open InteroperabilityProject/WPFClient in Visual Studio → Run  

### JAXB Validator
cd IISJaxbValidator  
javac -cp . src/*.java  
java -cp . Main  

### XML-RPC Server
cd IISXmlRpcServer  
dotnet run  

---

## 🔐 Security Notes
- All keys + DB strings removed  
- Add your own credentials  
- Use environment variables or user secrets  

---

## 📌 Short Description (for pinned repo)
Full IIS project with REST XSD/RNG validation, SOAP XPath filtering, Java JAXB validator, C# XML-RPC server, and WPF desktop client.

---

## 📄 License
Educational use only (IIS course).
