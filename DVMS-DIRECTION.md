# DVMS direction — Grant's decision, September 24, 2026

## Current priority

Customize and finish Grant's existing Ghost websites. Determine the near-term implementation of forms, quizzes/assessments, saved responses, and graphical results. Possible additional features include post likes and page visitor tracking. Identity/access for Ghost-related operations must be considered when that work is scoped.

The Request Desk screenshot supplied in this conversation is the reference for a simple, polished operations workspace. A comprehensive admin framework is not a prerequisite for these Ghost extensions.

## When returning to the DVMS Web Framework

Remind Grant of this decision before proposing framework changes:

**Keep Blazor and the existing DevExpress-based DVMS implementation. Improve its UI rather than rebuild it in Bootstrap/HTMX merely to achieve a modern appearance.**

Retain the advanced grid, TreeList (including editing/drag-and-drop requirements), and other needed components. Explore these four DevExpress themes as the visual foundations:

- Blazing Berry
- Blazing Dark
- Office White
- Purple

Grant identified that these themes include Bootstrap CSS. DevExpress's current documentation confirms that its Classic themes include both DevExpress and Bootstrap CSS rules and support SCSS customization. Check the actual Blazor project/package version before applying version-specific instructions. Grant has stated that his DevExtreme license covers 25.1.13; do not infer the existing Blazor project's version from that alone.

Reference: https://docs.devexpress.com/Blazor/404821/styling-and-themes/styling-and-themes

Improve layout, typography, spacing, navigation, cards, icons, and component styling within the existing application. Bootstrap support does not imply that an entire third-party theme can be imported without compatibility work.

## Deferred goals

1. Replacing Ghost itself.
2. A general-purpose portal platform for future SaaS and internal applications.
3. A broader CMS with community, LMS, and store features.

Preserve an upgrade path through modular simplicity and encapsulated complexity, but do not build these deferred goals into the near-term Ghost extension work.

## Status of prototypes

This directory is the original HTMX dashboard experiment. The separate Sneat Dashboard/API experiment is also a prototype, not a decision to migrate the existing Blazor framework. No further migration or implementation is authorized merely by this decision note.

Grant explicitly asked to be reminded of this direction when returning to DVMS because remembering prior decisions is difficult. This note is stored in this experiment workspace; it is not a global reminder or a scheduled automation.
