import React, { useMemo } from "react";
import ReactSelect from "react-select";

const PlanProcedureItem = ({planProcedure, users, onAssignedUsersChange}) => {

    const selectedUsers = useMemo(
        () => (planProcedure.assignedUsers || []).map((u) => ({
            label: u.user?.name,
            value: u.userId,
        })),
        [planProcedure.assignedUsers]
    );

    const handleAssignUsersToProcedure = async (selectedOptions) => {
        const nextUsers = selectedOptions || [];
        await onAssignedUsersChange(planProcedure, nextUsers);
    };

    return (
        <div className="py-2">
            <div className="d-flex justify-content-between align-items-center">
                <div>
                    {planProcedure.procedure.procedureTitle}
                </div>
            </div>

            <ReactSelect
                className="mt-2"
                placeholder="Select User to Assign"
                isMulti={true}
                options={users}
                value={selectedUsers}
                onChange={handleAssignUsersToProcedure}
            />
        </div>
    );
};

export default PlanProcedureItem;
