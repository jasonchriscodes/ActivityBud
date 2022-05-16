import React from "react";
import { Message } from "semantic-ui-react";

interface Props {
  errors: string[] | null;
}

export default function ValidationErrors({ errors }: Props) {
  return (
    <Message error>
      {errors && (
        <Message.List>
          {errors.map((err, i) => (
            <Message.Item key={i}>{err}</Message.Item> // index is used as key
          ))}
        </Message.List>
      )}
    </Message>
  );
}
