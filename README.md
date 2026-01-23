# Log File Analysis API

## Overview

A simple ASP.NET Core Minimal API that analyzes HTTP access log files and reports:

* Number of **unique IP addresses**
* **Top 3 most visited URLs**
* **Top 3 most active IP addresses**

---

## Tech Stack

* .NET / ASP.NET Core Minimal API
* C#
* xUnit

---

## API

### `POST /analyze`

Accepts a log file via `multipart/form-data` and returns an analysis summary.

Example response:

```json
{
  "uniqueIpCount": 11,
  "topUrls": ["/intranet-analytics/", "/login", "/reports"],
  "topIpAddresses": ["177.71.128.21", "168.41.191.40", "50.112.00.11"]
}
```

---

## Design Notes

* Uses **Minimal APIs** with endpoint mapping via extension methods
* Parsing logic is encapsulated in a **parser service**
* A **repository** handles file reading and delegates parsing
* Interfaces are used for testability; concrete implementations are `sealed`
* Anti-forgery is disabled for the file upload endpoint as this is a pure API

---

## Error Handling

* Validates required inputs (file presence, empty file)
* Malformed log lines are skipped
* Unexpected errors rely on ASP.NET Core’s default exception handling

---

## Assumptions

* The log file format is **consistent and known**
* Only IP address and URL are required from each log entry
* Files are of reasonable size (current implementation reads into memory)
* Supporting multiple log formats is out of scope

---

## Testing

* Unit tests target the **parser service**
* Tests cover empty input, single entry, and ranking logic

---
