# Shop Floor Inspection Process Flow

## Roles

| Role | Abbreviation | Responsibility |
|------|-------------|----------------|
| Quality Engineer | QE | Prepares IRS, writes dispositions, observes results |
| Manufacturing Engineer | ME | Reviews and approves IRS |
| Quality Team Leader | QTL | Reviews and approves IRS |
| Inspector | — | Executes inspection, enters measurements, records defects, manages status |

---

## Phase 1 — IRS Preparation & Approval

1. **QE prepares the Inspection Recording Sheet (IRS)**
   - Contains blue print screenshots, specification references, model images
   - Contains inspection tables with characters defined as either:
     - **Variable** — numeric measurement against tolerances (min/max)
     - **Attribute** — conformance check (pass/fail per criterion)
   - Last page contains a **Revision History Table**

2. **IRS sent for approval** to Manufacturing Engineer + Quality Team Leader

3. **After approval** — IRS placed in shared link folder (accessible to Inspector)

---

## Phase 2 — Inspection Session Start

Triggered when the manufacturing part arrives at the inspection operation defined on the **shop router**.

1. Inspector opens **QualiSight Inspection Program**
2. Inspector enters all required information to create the inspection session:
   - Part Number, Serial Number, Operation Number
   - Inspector name
   - Links to relevant project/IRS
3. **Inspection record is generated** (status: `open`)

> **Note:** An inspection session may only be started and closed by an Inspector. Engineers are observers with read-only access during this phase.

---

## Phase 3 — Dimensional Inspection

Inspector navigates to the **Dimensional Zone** and works through characters in IRS table order.

### Per Character Entry

Each character entry may involve up to four sub-sections:

| Section | Purpose |
|---------|---------|
| **Parts** | Router may contain plural parts — each part's measurement is saved separately |
| **Zones** | B/P may define special zones for a dimension — each zone value is recorded separately |
| **Notes** | Free-text for inspector remarks: unusual findings, clarifications, anything not anticipated |
| **Manual Accept / Manual Reject** | Inspector has the right to override program judgment and manually classify a character as Conform or Not Conform |

### Measurement Validation (on Save)

When inspector hits **Save / Enter**:

1. System checks every entered value against drawing tolerances
2. Tolerance limits were calculated at the start of the inspection process
3. The **IRS is converted to HTML** and rendered live during inspection — showing the original IRS layout with actual values filled in
4. Result is written as `Min / Max` into the entry box and the HTML IRS view
5. **Out-of-tolerance values are highlighted**

> For **attribute characters**: all entered values are indicated (no numeric tolerance check — conformance is per the defined criterion)

### Dimensional Inspection Reports (generated after inspection)

- **Filled IRS** — Original IRS with actual values populated in measurement cells. Out-of-tolerance values highlighted in yellow. **Critical constraint: original shape, size, and form of IRS must be preserved exactly — only actual value cells filled, yellow highlights on non-conforming entries.**
- **Detail Report** — Dimensional detail data: zones, parts breakdown, notes per character

---

## Phase 4 — Visual Inspection

Inspector examines the part surface for anomalies:

- Dent, nick, scratch, deformation, and other surface defects

### Per Defect

1. Inspector selects **defect type**
2. Enters defect dimensions / attributes (depth, width, length, color, etc.)
3. **Takes photo** of the defect and **marks it** using annotation tools — or marks first then photographs
4. Defect is saved and linked to the inspection

### Visual Inspection Report

- **Visual Report** — Compiled list of all defects with photos, annotations, measurements, and disposition history

---

## Phase 5 — Reporting Summary

After inspection completion, three report types are needed:

| # | Report | Content | Key Constraint |
|---|--------|---------|----------------|
| 1 | **Filled IRS** | Original IRS with actual measured values, yellow highlight on non-conforming | Must preserve original IRS layout exactly — no reformatting |
| 2 | **Dimensional Detail Report** | Zones, parts, notes per character | Full breakdown per measurement entry |
| 3 | **Visual Report** | Defects, photos, annotations, disposition trail | Full visual nonconformance record |

---

## Phase 6 — Engineering Disposition

After inspection, **QE reviews results** and writes dispositions for all nonconformances (both dimensional and visual).

### Disposition Scenarios (example)

| Defect | QE Decision | Next Action |
|--------|-------------|-------------|
| Defect A | **Use As Is / Accept per B/P** | No further action needed — inspector marks closed |
| Defect B | **Rework** → Re-Inspect | Part goes to rework station, then **returns directly to inspection station** |
| Defect C | **Submit to MRB** | Escalated to Material Review Board for decision |

### Inspector Post-Disposition Actions

1. Inspector checks QE's dispositions on the system
2. **Updates each defect status** according to the disposition applied
3. For **Rework → Re-Inspect** cases: part returns to inspection, inspector performs re-check and records result as a **child defect** linked to the original
4. Once **no open defects remain**, inspector has the right to change inspection status to **`completed`**

> **Rule:** Inspector cannot close inspection if any nonconforming character or defect lacks a final neutralizing disposition (USE_AS_IS, SCRAP, MRB_ACCEPTED, MRB_REJECTED, REPAIR, or REWORK resolved via re-inspection).

---

## Status Transition Summary

```
open → (inspection in progress)
     → completed   [Inspector closes — all nonconformances resolved]
     → rejected    [Special case — not normal flow]
```

---

## Key Process Rules

- IRS is the authoritative inspection reference document — prepared and approved before any inspection begins
- Inspector owns the inspection lifecycle from start to close
- Engineer is an observer during inspection; writes dispositions after results are available
- Dispositions reflect paper-based decisions made by QE — system records them, not decides them (current maturity)
- Re-inspected parts after rework go **directly back to inspection station** — no intermediate steps
- All three report types must be generated before inspection can be considered complete
- Filled IRS report must maintain exact original document fidelity
